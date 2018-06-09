using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using RAIDAChatNode.DTO;
using Newtonsoft.Json;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;

namespace RAIDAChatNode.Utils
{
    public static class DeserializeObject
    {
        public static T ParseJSON<T> (object json, OutputSocketMessage outp, out OutputSocketMessageWithUsers rez)
        {
            rez = new OutputSocketMessageWithUsers();
            T sucObj = default(T);
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                sucObj = JsonConvert.DeserializeObject<T>(json.ToString(), settings);
            }
            catch
            {
                outp.success = false;
                outp.msgError = "Invalid sending data";
                rez.msgForOwner = outp;
            }
            return sucObj;
        }

       
        public static void IsValid(object model)
        {
            var validCOntext = new ValidationContext(model, null, null);
            Validator.ValidateObject(model, validCOntext, true);
        }
       
        /// <summary>
        /// Get all users in dialog with me
        /// </summary>
        /// <param name="db"></param>
        /// <param name="user"></param>
        /// <returns>List of user logins</returns>
        public static List<string> GetMyReferenceUser(RaidaContext db, Members user)
        {
            List<string> users = new List<string>();
            user.MemberInGroup.ToList().ForEach(it =>
            {
                db.MemberInGroup.Include(m => m.member).Where(mig => mig.groupId.Equals(it.groupId))
                    .ForEachAsync(mem => users.Add(mem.member.login));
            });
            users = users.Distinct().Where(it => !it.Equals(user.login)).ToList();
            return users;
        }
    }
}
