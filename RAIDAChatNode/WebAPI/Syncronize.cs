using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
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
                    object rez = null;
                    
                    if (data.Name.Equals(SyncData.GetHash(typeof(Members))))
                    {
                        var SyncData = db.Members.Select(it => new
                        {
                            it.description_fragment,
                            it.last_use,
                            it.login,
                            it.nick_name,
                            it.pass,
                            it.online,
                            it.photo_fragment,
                            groups = it.MemberInGroup.Where(gr => gr != null && gr.member.Equals(it)).Select(g => g.groupId).ToList()
                        }).ToList(); 
                        var EqualsHash = SyncData.Select(it => new {it.login, it.nick_name, it.groups}).ToList();
                        Console.WriteLine(JsonConvert.SerializeObject(EqualsHash[0]));
                        rez = CollectionData<SyncMemberInfo>(EqualsHash, data.Rows, SyncData);    
                    }
                    else if (data.Name.Equals(SyncData.GetHash(typeof(Groups))))
                    {
                        var SyncData = db.Groups.Select(it => new
                            {
                                it.group_id,
                                it.group_name_part,
                                it.one_to_one,
                                it.photo_fragment,
                                it.privated,
                                owner = it.MemberInGroup.First(gr => gr.group.Equals(it)).member.login
                            }).ToList();
                        var EqualsHash = SyncData.Select(it => new {it.group_id, it.group_name_part, it.one_to_one}).ToList();
                        rez = CollectionData<SyncGroupInfo>(EqualsHash, data.Rows, SyncData);
                    }
                    else
                    {
                        return NotFound();
                    }

                    var objRez = CryptoUtils.EncryptDataAES(rez);
                    objRez.key = CryptoUtils.EncryptKeyRSA(data.PublicKey, objRez.key);
                    
                    return new ObjectResult(new {objRez.key, objRez.code});
                }
                 
            }

            return NotFound();
        }

        /// <summary>
        /// Separating actuals and new datas
        /// </summary>
        /// <param name="EqualHash"></param>
        /// <param name="act">List of hashes of actual data</param>
        /// <param name="Syncroned">List of all information about new object</param>
        /// <typeparam name="T">Syncronized type of object</typeparam>
        /// <returns>List of hashes of actual rows and List for sync data</returns>
        private object CollectionData<T>(IList EqualHash, List<string> act, IList Syncroned) where T : class
        {
            List<T> newRows = new List<T>();
            List<string> actual = new List<string>();
            
            for (int i = 0; i < EqualHash.Count; i++)
            {
                string rowHash = SyncData.GetHash(EqualHash[i]);
                if (act.Contains(rowHash))
                {
                    actual.Add(rowHash);
                }
                else
                {
                    newRows.Add(JsonConvert.DeserializeObject<T>( JsonConvert.SerializeObject(Syncroned[i])));
                }
            }

            return new {Actual = actual, NewRows = newRows};
        }        
    }
}