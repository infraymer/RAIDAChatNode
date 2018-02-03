using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Utils;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace RAIDAChatNode.Reflections
{
    public class SetDialogPrivate : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin)
        {
            #region Тестовые данные
            /*
            { 
                "execFun": "SetDialogPrivate", 
                "data": {
                    "publicId": "18A0CA0657DE4FB09CDC86008B2A8EBE",
                    "privated": "false"  
                }  
            }
            */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("SetDialogPrivate", true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            SetDialogPrivateInfo info = DeserializeObject.ParseJSON<SetDialogPrivateInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {
                Members owner = db.Members.First(it => it.login.Equals(myLogin));
                if(db.Groups.Any(it => it.group_id == info.publicId && it.owner == owner))
                {
                    Groups group = db.Groups.First(it => it.group_id == info.publicId && it.owner == owner);
                    group.privated = info.privated;
                    db.SaveChanges();
                    output.data = new { dialogId = group.group_id, group.privated };

                    List<MemberInGroup> membersInGroup = db.MemberInGroup.Include(m => m.member).Where(it => it.group == group && it.member != owner).ToList();
                    membersInGroup.ForEach(it => rez.forUserLogin.Add(it.member.login));
                    rez.msgForOther = output;
                }
                else
                {
                    output.success = false;
                    output.msgError = "This dialog not found or you not owner";
                    rez.msgForOwner = output;
                    return rez;
                }
                
            }


            rez.msgForOwner = output;
            return rez;
        }
    }
}
