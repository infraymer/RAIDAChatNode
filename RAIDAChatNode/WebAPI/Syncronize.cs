using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;
using RAIDAChatNode.Utils;
using RAIDAChatNode.WebAPI.DTO;

namespace RAIDAChatNode.WebAPI
{
    [Route("api/[controller]")]
    public class SyncController : Controller
    {
        [HttpPost]
        public IActionResult Start([FromBody] OneTableHash data)
        {
            if(ModelState.IsValid)
            {
                using (var db = new RaidaContext())
                {
                    List<SyncMemberInfo> syncData = db.Members.Select(it => new SyncMemberInfo()
                    {
                        Descript = it.description_fragment,
                        LastUse = it.last_use,
                        Login = it.login,
                        NickName = it.nick_name,
                        Password = it.pass,
                        Online = it.online,
                        Photo = it.photo_fragment,
                        Hash = SyncData.GetHash(it)
                    }).ToList();
                    var memberInfo = CollectionData(data.ActualMembers, syncData);

                    List<SyncGroupInfo> syncDataGroup = db.Groups.Select(it => new SyncGroupInfo()
                    {
                        Id = it.group_id,
                        Name = it.group_name_part,
                        PeerToPeer = it.one_to_one,
                        Photo = it.photo_fragment,
                        Privated = it.privated,
                        Owner = it.owner.login,
                        Hash = SyncData.GetHash(it)
                    }).ToList();
                    var groupInfo = CollectionData(data.ActualGroups, syncDataGroup);


                    List<SyncMembInGroupInfo> syncDataMiG = db.MemberInGroup.Include(m => m.member).ToList().Select(
                        it =>
                            new SyncMembInGroupInfo()
                            {
                                GroupId = it.groupId,
                                Login = it.member.login,
                                Hash = SyncData.GetHash(it)
                            }).ToList();
                    var mingInfo = CollectionData(data.ActualMinG, syncDataMiG);

                    var objRez = CryptoUtils.EncryptDataAES(new {memberInfo, groupInfo, mingInfo});
                    objRez.key = CryptoUtils.EncryptKeyRSA(data.PublicKey, objRez.key);

                    return new ObjectResult(new {objRez.key, objRez.code});

                }
            }

            return NotFound();
        }

        [HttpGet]
        [Route("itself")]
        public void Itself()
        {
            new SyncData().Sync();   
        }
        
        /// <summary>
        /// Separating actuals and new datas
        /// </summary>
        /// <param name="actualList">List of hashes of actual data</param>
        /// <param name="syncroned">List of all information about new object</param>
        /// <typeparam name="T">Syncronized type of object</typeparam>
        /// <returns>List of hashes of actual rows and List for sync data</returns>
        private object CollectionData<T>(List<string> actualList, List<T> syncroned) where T : class, ICollcetionObject
        {
            List<T> newRows = new List<T>();
            List<string> actual = new List<string>();

            syncroned.ForEach(it =>
            {
                if (actualList.Contains(it.Hash))
                {
                    actual.Add(it.Hash);
                }
                else
                {
                    newRows.Add(JsonConvert.DeserializeObject<T>( JsonConvert.SerializeObject(it)));
                }    
            });
            return new {Actual = actual, NewRows = newRows};
        } 
        
    }
}