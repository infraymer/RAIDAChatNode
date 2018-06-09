using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;
using RAIDAChatNode.Utils;

namespace RAIDAChatNode.Reflections
{
    public class ChangeUserInfo : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin)
        {
              #region Тестовые данные
            /* { 
                    "execFun": "ChangeUserInfo", 
                    "data": {
                        "name":"String",
						"photo":"String",
						"about":"String",
						"changePass":"Boolean",
						"oldPass":"String",
						"newPass":"String"
                    }
                }
            */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("changeUserInfo", true, "", new { });
            OutputSocketMessage outputForOther = new OutputSocketMessage("changeUserInfo", true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            DTO.ChangeUserInfo info = DeserializeObject.ParseJSON<DTO.ChangeUserInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {

                Members owner = db.Members.Include(m=>m.MemberInGroup).FirstOrDefault(it => it.login.Equals(myLogin));

                owner.nick_name = info.Name;
                owner.photo_fragment = info.Photo;
                owner.description_fragment = info.About;
                if (info.ChangePass)
                {
                    if (Argon2.Verify(owner.pass, info.OldPass, null))
                    {
                        if (info.NewPass.Trim().Length > 5)
                        {
                            owner.pass = Argon2.Hash(info.NewPass, 1, 512, 8);    
                        }
                        else
                        {
                            output.success = false;
                            output.msgError = "Minimum length of password is 6 chars";
                        }
                    }
                    else
                    {
                        output.success = false;
                        output.msgError = "Current password is not valid";
                    }
                       
                }

                db.SaveChanges();

                var user = new UserInfo(owner.login, owner.nick_name, owner.photo_fragment, owner.online);
                
                output.data = new
                {
                    itself = true,
                    user
                };

                outputForOther.data = new
                {
                    itself = false,
                    user
                };

                rez.forUserLogin = DeserializeObject.GetMyReferenceUser(db, owner);

            }

            rez.msgForOwner = output;
            rez.msgForOther = outputForOther;
            return rez;
        }
    }
}