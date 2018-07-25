using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;

namespace RAIDAChatNode.Reflections
{
    public class GetUsersInfo : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin, Guid actId)
        {
            #region Тестовые данные
            /* { 
                    "execFun": "GetUsers", 
                    "data": { }
                }
            */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("getUsersInfo", actId, true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            using (var db = new RaidaContext())
            {
              
                List<MemberInGroup> mg = db.MemberInGroup.Include(g => g.group)
                    .ThenInclude(own => own.owner)
                    .Where(it => it.member.login.Equals(myLogin))
                    .ToList();

                List<UserInfo> users = new List<UserInfo>();
                
                mg.ForEach(it => {
                    db.MemberInGroup.Include(m => m.member).Where(mig => mig.groupId.Equals(it.groupId))
                        .ForEachAsync(memb =>
                        {
                         users.Add(new UserInfo(
                             memb.member.login,
                             memb.member.nick_name,
                             memb.member.photo_fragment.ToString(),
                             memb.member.online));   
                        });
                });

                users = users.Distinct().ToList();

                output.data = users;
            }

            rez.msgForOwner = output;
            return rez;
        }
    }
}