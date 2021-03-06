﻿using System;
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
    public class AddMemberInGroup : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin, Guid actId)
        {
            #region Тестовые данные
            /*
                        {
                            "execFun": "addMemberInGroup",
                            "data": {
                                "memberLogin": "shRek1",
                                "groupId": "48A0CA0657DE4FB09CDC86008B2A8EBE",
                                 "transactionId": "80f7efc032dd4a7c97f69fca51ad1100"
                            }
                        }
                            */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("addMemberInGroup", actId, true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            AddMemberInGroupInfo info = DeserializeObject.ParseJSON<AddMemberInGroupInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {
                if (db.Members.Any(it => it.login.Equals(info.memberLogin.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    Members owner = db.Members.First(it => it.login.Equals(myLogin));
                    if (db.MemberInGroup.Any(it => it.member == owner && it.groupId == info.groupId && !it.group.one_to_one)) 
                    {
                        Members user = db.Members.FirstOrDefault(it => it.login.Equals(info.memberLogin.Trim(), StringComparison.CurrentCultureIgnoreCase));
                        if (db.MemberInGroup.Any(it => it.groupId == info.groupId && it.member == user))
                        {
                            output.success = false;
                            output.msgError = "This user already exists in this group";
                        }
                        else
                        {
                            Groups group = db.Groups.First(it => it.group_id == info.groupId);
                           
                            if (group.privated)
                            {
                                output.success = false;
                                output.msgError = "This group is private";
                                rez.msgForOwner = output;
                                return rez;
                            }

                            int newId = 0;
                            if (db.MemberInGroup.Any())
                            {
                                newId = db.MemberInGroup.OrderByDescending(it => it.Id).FirstOrDefault().Id + 1;
                            }

                            MemberInGroup mg = new MemberInGroup
                            {
                                Id = newId,
                                group = group,
                                member = user
                            };
                            db.MemberInGroup.Add(mg);

                            Transaction.saveTransaction(db, info.transactionId, Transaction.TABLE_NAME.MEMBERS_IN_GROUP, mg.Id.ToString(), owner);

                            db.SaveChanges();
                            
                            output.data = new {itself = true, newUser = new UserInfo(user.login, user.nick_name, user.photo_fragment, user.online), groupId = group.group_id};
                            
                            var members = db.MemberInGroup.Include(ming => ming.member)
                                .Where(it => it.groupId.Equals(group.group_id)).Select(x => x.member.login).ToList(); //Logins of all users in dialog
                            rez.forUserLogin = members.Where(it => !it.Equals(myLogin)).ToList(); //Send all users in dialog, but not me
                            
                            rez.msgForOther = new
                            {
                                callFunction = "AddMemberInGroup",
                                data = new {
                                    id = info.groupId,
                                    name = group.group_name_part,
                                    oneToOne = group.one_to_one,
                                    members = members,
                                    newUser = new UserInfo(user.login, user.nick_name, user.photo_fragment, user.online)
                                }
                            };
                        }
                    }
                    else
                    {
                        output.success = false;
                        output.msgError = "Group is not found";
                    }
                }
                else
                {
                    output.success = false;
                    output.msgError = "User is not found";
                }
            }

            rez.msgForOwner = output;
            return rez;
        }
    }
}
