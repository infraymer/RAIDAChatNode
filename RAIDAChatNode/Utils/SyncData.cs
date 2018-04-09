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
            List<InputInfo> syncDataList = new List<InputInfo>();
            (privateKey, publicKey) = CryptoUtils.GenerateRSAKeys();
            
            using (var db = new RaidaContext())
            {
                OneTableHash actualInfo = new OneTableHash() {PublicKey = publicKey};
                
                await db.Members.Include(it=>it.MemberInGroup).ForEachAsync(it => {
                    actualInfo.ActualMembers.Add(GetHash(it));
                });

                await db.Groups.ForEachAsync(it => {
                    actualInfo.ActualGroups.Add(GetHash(it));
                });

                await db.MemberInGroup.ForEachAsync(it => {
                    actualInfo.ActualMinG.Add(GetHash(it));
                });
                
                try
                {
                    foreach (string serverUrl in MainConfig.TrustedServers)
                    {
                        InputSyncData responseMember = await $"{serverUrl}/api/sync".PostJsonAsync(actualInfo).ReceiveJson<InputSyncData>();                   
                        InputInfo tmpInfo = DecryptResponse<InputInfo>(responseMember);
                        tmpInfo.ServerURL = serverUrl;
                        syncDataList.Add(tmpInfo);
                    }   
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    Console.ReadLine();
                }

                if (syncDataList.Count > 0)
                {
                    InputInfo correctData = IntersectData(syncDataList);
                    SyncronizeBase(correctData, db);
                    DetectIncorrectServer(correctData, syncDataList);    
                }
            }
        }

        /// <summary>
        /// Return SHA256 hash for object
        /// </summary>
        /// <param name="obj">Input object</param>
        /// <returns>String base64</returns>
        public static string GetHash<T>(T obj)
        {
            object tmpObj;
            switch (typeof(T).FullName)
            {
                case "RAIDAChatNode.Model.Entity.Members1":
                    Members tmp = obj as Members;
                    tmpObj = new {tmp?.login, tmp?.nick_name}; 
                    break;
                case "RAIDAChatNode.Model.Entity.Groups":
                    Groups tmp1 = obj as Groups;
                    tmpObj = new {tmp1?.group_id, tmp1?.group_name_part, tmp1?.one_to_one};
                    break;
                case "RAIDAChatNode.Model.Entity.MemberInGroup":
                    MemberInGroup tmp2 = obj as MemberInGroup;
                    Console.WriteLine(tmp2?.member?.login);
                    tmpObj = new {tmp2?.groupId, tmp2?.member?.login};
                    break;
                default:
                    tmpObj = obj;
                    break;
            }
            
            SHA256 sha = SHA256.Create();
            return Convert.ToBase64String(
                    sha.ComputeHash(
                        Encoding.Unicode.GetBytes(
                            JsonConvert.SerializeObject(tmpObj, new JsonSerializerSettings(){ReferenceLoopHandling = ReferenceLoopHandling.Ignore})
                        )
                    )
            );
        }

        private T DecryptResponse<T>(InputSyncData encryptedData)
        {
            CryptoUtils.SecretAESKey dataKey = CryptoUtils.DecryptSecretAESKey(privateKey, encryptedData.Key);
            T decryptData = CryptoUtils.DecryptDataAES<T>(dataKey, encryptedData.Code);
            return decryptData;
        }
        
        
      
        /// <summary>
        /// Returns only the same records from servers
        /// </summary>
        /// <param name="inputData">List responses from trusted servers</param>
        /// <returns></returns>
        private InputInfo IntersectData(List<InputInfo> inputData)
        {

            InputInfo correctData = new InputInfo();
            if (inputData.Count > 0)
            {   
                correctData.MemberInfo = inputData[0].MemberInfo;
                correctData.GroupInfo = inputData[0].GroupInfo;
                correctData.MinGInfo = inputData[0].MinGInfo;
                for (int i = 1; i < inputData.Count - 1; i++)
                {
                    correctData.MemberInfo.Actual = correctData.MemberInfo.Actual.Intersect(inputData[i].MemberInfo.Actual).ToList();
                    correctData.MemberInfo.NewRows = correctData.MemberInfo.NewRows.Intersect(inputData[i].MemberInfo.NewRows).ToList();
                    
                    correctData.GroupInfo.Actual = correctData.GroupInfo.Actual.Intersect(inputData[i].GroupInfo.Actual).ToList();
                    correctData.GroupInfo.NewRows = correctData.GroupInfo.NewRows.Intersect(inputData[i].GroupInfo.NewRows).ToList();
                    
                    correctData.MinGInfo.Actual = correctData.MinGInfo.Actual.Intersect(inputData[i].MinGInfo.Actual).ToList();
                    correctData.MinGInfo.NewRows = correctData.MinGInfo.NewRows.Intersect(inputData[i].MinGInfo.NewRows).ToList();
                }    
            }
            return correctData;
        }


        private void DetectIncorrectServer(InputInfo correctInfo, List<InputInfo> trustedInfos)
        {
            trustedInfos.ForEach(it =>
            {
                if ((correctInfo.MemberInfo.Actual.Count != (it.MemberInfo.Actual.Count + it.MemberInfo.NewRows.Count))
                    || (correctInfo.GroupInfo.Actual.Count != (it.GroupInfo.Actual.Count + it.GroupInfo.NewRows.Count))
                    || (correctInfo.MinGInfo.Actual.Count != (it.MinGInfo.Actual.Count + it.MinGInfo.NewRows.Count)))
                {
                    $"{it.ServerURL}/api/sync/itself".GetAsync();
                } 
            });
        }

        private void SyncronizeBase(InputInfo correctData, RaidaContext db)
        {
            //Save\Update members table
            correctData.MemberInfo.NewRows.ForEach(it =>
            {
                correctData.MemberInfo.Actual.Add(GetHash(new {login = it.Login, nick_name = it.NickName}));
                if (db.Members.Any(m => m.login.Equals(it.Login)))
                {
                    //UPDATE
                    Members member = db.Members.First(m => m.login.Equals(it.Login));
                    member.nick_name = it.NickName;
                    member.description_fragment = it.Descript;
                    member.last_use = it.LastUse;
                    member.online = it.Online;
                    member.photo_fragment = it.Photo;
                }
                else
                {
                    //INSERT NEW
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
                }
            });
            db.SaveChanges();
            
            
            //Save\Update groups table
            correctData.GroupInfo.NewRows.ForEach(it =>
            {
                correctData.GroupInfo.Actual.Add(GetHash(new {group_id = it.Id, group_name_part = it.Name, one_to_one = it.PeerToPeer}));
                Members owner = db.Members.First(m => m.login.Equals(it.Owner));
                if (db.Groups.Any(g => g.group_id == it.Id))
                {
                    Groups group = db.Groups.First(g => g.group_id == it.Id);
                    group.group_name_part = it.Name;
                    group.one_to_one = it.PeerToPeer;
                    group.photo_fragment = it.Photo;
                    group.privated = it.Privated;
                    group.owner = owner;
                }else
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
            
            //Save member in groups
            correctData.MinGInfo.NewRows.ForEach(it =>
            {
                correctData.MinGInfo.Actual.Add(GetHash(new {groupId = it.GroupId, login = it.Login}));
                if (db.Members.Any(m => m.login.Equals(it.Login) && db.Groups.Any(g => g.group_id == it.GroupId)))
                {
                    Groups group = db.Groups.First(gr => gr.group_id == it.GroupId);
                    Members member = db.Members.First(m => m.login.Equals(it.Login));

                    if (!db.MemberInGroup.Any(mig => mig.member.Equals(member) && mig.group.Equals(group))) //Find duplicate
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
            db.SaveChanges();
            
            //Remove not actual data in tables
            db.Members.ForEachAsync(it =>
            {
                if (!correctData.MemberInfo.Actual.Contains(GetHash(it)))
                {
                    db.Members.Remove(it);
                }
            });
            
            db.Groups.ForEachAsync(it =>
            {
                if (!correctData.GroupInfo.Actual.Contains(GetHash(it)))
                {
                    db.Groups.Remove(it);
                }
            });
            
            db.MemberInGroup.ForEachAsync(it =>
            {
                if (!correctData.MinGInfo.Actual.Contains(GetHash(it)))
                {
                    db.MemberInGroup.Remove(it);
                }
            });
            db.SaveChangesAsync();
        }
    }
}