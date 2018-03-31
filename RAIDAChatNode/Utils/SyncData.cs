using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using Flurl.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RAIDAChatNode.DTO;
using RAIDAChatNode.DTO.Configuration;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;
using RAIDAChatNode.WebAPI.DTO;

namespace RAIDAChatNode.Utils
{
    public class SyncData
    {
        private List<Members> originalMembers;
        private List<Groups> originalGroups;
        private RSAParameters privateKey, publicKey;
        
        public async void Sync()
        {
            List<DataInfo<SyncMemberInfo>> syncDataList = new List<DataInfo<SyncMemberInfo>>();
            List<DataInfo<SyncGroupInfo>> syncGroupDataList = new List<DataInfo<SyncGroupInfo>>();
            (privateKey, publicKey) = CryptoUtils.GenerateRSAKeys();
            
            using (var db = new RaidaContext())
            {
                OneTableHash membersInfo = new OneTableHash() { Name = GetHash(typeof(Members)), PublicKey = publicKey};
                OneTableHash groupsInfo = new OneTableHash() { Name = GetHash(typeof(Groups)), PublicKey = publicKey };
                
                originalMembers = db.Members.Include("MemberInGroup").ToList();
                var data = originalMembers.Select(it => new
                {
                    it.login, 
                    it.nick_name, 
                    groups = it.MemberInGroup.Where(gr => gr != null && gr.member.Equals(it)).Select(g => g.groupId).ToList()
                }).ToList();
                if (data.Count > 0)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(data[0]));    
                }
                
                data.ForEach(it => 
                {
                    membersInfo.Rows.Add(GetHash(it));
                });

                originalGroups = db.Groups.ToList();
                var dataGr = originalGroups.Select(it => new {it.group_id, it.group_name_part, it.one_to_one}).ToList();
                
                dataGr.ForEach(it => 
                {
                    groupsInfo.Rows.Add(GetHash(it));
                });
                
                try
                {
                    foreach (string ServerUrl in MainConfig.TrustedServers)
                    {
                        InputSyncData responseMember = await $"{ServerUrl}/api/sync".PostJsonAsync(membersInfo).ReceiveJson<InputSyncData>();                   
                        DataInfo<SyncMemberInfo> infoMember = DecryptResponse<SyncMemberInfo>(responseMember);
                        syncDataList.Add(infoMember);
                    
                        InputSyncData responseGroup = await $"{ServerUrl}/api/sync".PostJsonAsync(groupsInfo).ReceiveJson<InputSyncData>();                   
                        DataInfo<SyncGroupInfo> infoGroup = DecryptResponse<SyncGroupInfo>(responseGroup);
                        syncGroupDataList.Add(infoGroup);  
                    }   
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    Console.ReadLine();
                }

                DataInfo<SyncMemberInfo> intersectDataMember = intersectData(syncDataList);
                DataInfo<SyncGroupInfo> intersectDataGroup = intersectData(syncGroupDataList);
               
                SyncronizeBase(intersectDataMember, intersectDataGroup, db);
            }
        }

        /// <summary>
        /// Return SHA256 hash for object
        /// </summary>
        /// <param name="obj">Input object</param>
        /// <returns>String base64</returns>
        public static string GetHash(object obj)
        {
            SHA256 sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(obj))));
        }

        private DataInfo<T> DecryptResponse<T>(InputSyncData encryptedData)
        {
            SecretAESKey DataKey = CryptoUtils.DecryptSecretAESKey(privateKey, encryptedData.Key);
            DataInfo<T> DecryptData = CryptoUtils.DecryptDataAES<DataInfo<T>>(DataKey, encryptedData.Code);
            return DecryptData;
        }
        
        
      
        /// <summary>
        /// Returns only the same records from servers
        /// </summary>
        /// <param name="inputData">List responses from trusted servers</param>
        /// <typeparam name="T">EF tableclass</typeparam>
        /// <returns></returns>
        private DataInfo<T> intersectData<T>(List<DataInfo<T>> inputData)
        {
            DataInfo<T> intersectData = null;
            if (inputData.Count > 0)
            {   
                intersectData = inputData[0];
                for (int i = 1; i < inputData.Count - 1; i++)
                {
                    intersectData.Actual = intersectData.Actual.Intersect(inputData[i].Actual).ToList();
                    intersectData.NewRows = intersectData.NewRows.Intersect(inputData[i].NewRows).ToList();
                }    
            }
            return intersectData;
        }


        private void DetectIncorrectServer()
        {
            //TODO: Обнаружение несовпадения данных на доверенных серверах и запуск их синхронизации
        } 
        
        
        private void SyncronizeBase(DataInfo<SyncMemberInfo> members, DataInfo<SyncGroupInfo> groups, RaidaContext db)
        {

            if (members != null)
            {
                originalMembers?.ForEach(it =>
                {
                    if (!members.Actual.Contains(GetHash(new {it.login, it.nick_name})))
                    {
                        db.Members.Remove(it);
                        it.MemberInGroup.ToList().ForEach(mig => { db.MemberInGroup.Remove(mig); });
                    }
                });
                db.SaveChanges();

                members.NewRows.ForEach(it =>
                {
                    Guid privateId = Guid.NewGuid();
                    while (db.Members.Any(memb => memb.private_id == privateId))
                    {
                        privateId = Guid.NewGuid();
                    }
                        
                    Members member = new Members
                    {
                        private_id = privateId,
                        login = it.Login.Trim().ToLower(),
                        pass = it.Password,
                        nick_name = it.NickName,
                        last_use = it.LastUse,
                        description_fragment = it.Descript,
                        photo_fragment = it.Photo,
                        kb_bandwidth_used = 0,
                        online = it.Online,
                    };
                    db.Members.Add(member);
                });
                db.SaveChanges();
            }
            
            
            if (groups != null)
            {
                originalGroups?.ForEach(it =>
                {
                    if (!groups.Actual.Contains(GetHash(new {it.group_id, it.group_name_part, it.one_to_one})))
                    {
                        db.Groups.Remove(it);
                    }
                });
                db.SaveChanges();

                groups.NewRows.ForEach(it =>
                {
                    Members owner = db.Members.First(m => m.login.Equals(it.Owner));
                    if (owner != null)
                    {
                        Groups newGroup = new Groups()
                        {
                            group_id = it.Id,
                            group_name_part = it.Name,
                            owner = owner,
                            photo_fragment = it.Photo,
                            one_to_one = it.PeerToPeer,
                            privated = it.Privated
                        };
                        db.Groups.Add(newGroup);
                    }
                });
                db.SaveChanges();
            }

            //Build relation Members <--> Groups
            if (members != null)
            {
                members.NewRows.ForEach(it =>
                {
                    it.Groups.ForEach(Id =>
                    {
                        if (db.Members.Any(m => m.login.Equals(it.Login) && db.Groups.Any(g => g.group_id == Id)))
                        {
                            Groups group = db.Groups.First(gr => gr.group_id == Id);
                            Members member = db.Members.First(m => m.login.Equals(it.Login));

                            if (!db.MemberInGroup.Any(mig => mig.member.Equals(member) && mig.group.Equals(group)))
                            {
                                int newId = 0;
                                if (db.MemberInGroup.Any())
                                {
                                    newId = db.MemberInGroup.OrderByDescending(mig => mig.Id).FirstOrDefault().Id + 1;
                                }

                                MemberInGroup mg = new MemberInGroup
                                {
                                    Id = newId,
                                    group = group,
                                    member = member
                                };
                                db.MemberInGroup.Add(mg);
                            }
                        }
                    });
                });
                db.SaveChanges();
            }            
        }
        
    }
}