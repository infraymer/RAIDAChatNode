using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Utils;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;

namespace RAIDAChatNode.Reflections
{
    public class CreateDialog : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin, Guid actId)
        {
            #region Тестовые данные
            /*
            {
                "execFun": "createDialog",
                "data": {
                    "name": "MeCloseGroup",
                    "publicId": "48A0CA0657DE4FB09CDC86008B2A8EBE",
                    "oneToOne": "false",
                    "transactionId": "80f7efc032dd4a7c97f69fca51ad3100"
                }
            }
            */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("createDialog", actId, true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            GroupInfo info = DeserializeObject.ParseJSON<GroupInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {
                if (db.Groups.Any(it => it.group_id == info.publicId))
                {
                    output.success = false;
                    output.msgError = "Change the publicId";
                    rez.msgForOwner = output;
                    return rez;
                }

                Members owner = db.Members.First(it => it.login.Equals(myLogin));
              
                Groups group = new Groups
                {
                    group_id = info.publicId,
                    group_name_part = info.name,
                    owner = owner,
                    photo_fragment = "",
                    one_to_one = info.oneToOne,
                    privated = false
                };

                int newId = 1;
                if ( db.MemberInGroup.Count() > 0 ){
                    newId = db.MemberInGroup.OrderByDescending(it => it.Id).FirstOrDefault().Id + 1;
                }
                

                if (info.oneToOne)
                {
                    if( db.Members.Any(it => it.login.Equals(info.name.Trim().ToLower())))
                    {
                        group.group_name_part = "OneToOne";
                        Members rec = db.Members.First(it => it.login.Equals(info.name.Trim().ToLower()));

                        if (!db.MemberInGroup.Any(it => it.member == owner && db.MemberInGroup.Any(mg => mg.member == rec && it.group == mg.group && mg.group.one_to_one)))
                        {

                            MemberInGroup mg1 = new MemberInGroup
                            {
                                Id = newId,
                                group = group,
                                member = rec
                            };
                            db.MemberInGroup.Add(mg1);

                            Transaction.saveTransaction(db, info.transactionId, Transaction.TABLE_NAME.MEMBERS_IN_GROUP, mg1.Id.ToString(), owner);

                            newId++;
                            //rez.usersId.Add(info.login); Придумать ответ пользователю, которого добавили - а может он и не нужен
                        }
                        else
                        {
                            output.success = false;
                            output.msgError = "This dialog exists";
                            rez.msgForOwner = output;
                            return rez;
                        }                            
                    }
                    else
                    {
                        output.success = false;
                        output.msgError = "User is not found";
                        rez.msgForOwner = output;
                        return rez;
                    }
                }
                db.Groups.Add(group);
                MemberInGroup OwnerInMg = new MemberInGroup
                {
                    Id = newId,
                    group = group,
                    member = owner
                };
                db.MemberInGroup.Add(OwnerInMg);           
                try {
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Transaction.saveTransaction(db, info.transactionId, Transaction.TABLE_NAME.GROUPS, info.publicId.ToString(), owner);
                Transaction.saveTransaction(db, info.transactionId, Transaction.TABLE_NAME.MEMBERS_IN_GROUP, OwnerInMg.Id.ToString(), owner);
                
            }
            output.data = new { id = info.publicId, info.name, info.oneToOne };
            rez.msgForOwner = output;
            return rez;
        }


    }
}
