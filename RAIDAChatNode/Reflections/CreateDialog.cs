using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Utils;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;

namespace RAIDAChatNode.Reflections
{
    public class CreateDialog : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin)
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

            OutputSocketMessage output = new OutputSocketMessage("createGroup", true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            GroupInfo info = DeserializeObject.ParseJSON<GroupInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {
                Members owner = db.Members.First(it => it.login.Equals(myLogin));

                //Условие на publicId, чтобы не было одинаковых ключей иначе ошибка
                Groups group = new Groups
                {
                    group_id = info.publicId,
                    group_name_part = info.name,
                    owner = owner,
                    photo_fragment = "",
                    one_to_one = info.oneToOne,
                    privated = false
                };

                int newId = 0;
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
                            //rez.usersId.Add(info.login); Придумать ответ пользователю, которого добавили
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

                Transaction.saveTransaction(db, info.transactionId, Transaction.TABLE_NAME.GROUPS, info.publicId.ToString(), owner);
                Transaction.saveTransaction(db, info.transactionId, Transaction.TABLE_NAME.MEMBERS_IN_GROUP, OwnerInMg.Id.ToString(), owner);
                try {
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
            output.data = new { id = info.publicId, info.name };
            rez.msgForOwner = output;
            return rez;
        }


    }
}
