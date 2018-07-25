using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;
using RAIDAChatNode.Utils;

namespace RAIDAChatNode.Reflections
{
    public class OrganizationCreate : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin, Guid actId)
        {

            #region Тестовые данные
            /*
                        {
                            "execFun": "OrganizationCreate",
                            "data": {
                                "publicId": "788FEFAD0ED24436AD73D968685110E8",     
                                "name": "Organization Only One",
                                "transactionId": "80f7efc092da4a1c47f69fca51ad1100"
                            }
                        }
                        */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("OrganizationCreate", actId, true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            OrganizationCreateInfo info = DeserializeObject.ParseJSON<OrganizationCreateInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {
                Members owner = db.Members.Include(o => o.organization).First(it => it.login.Equals(myLogin));
                if(owner.organization == null)
                {
                    if(!db.Organizations.Any(it => it.public_id == info.publicId))
                    {
                        Guid privateId = Guid.NewGuid();
                        while(db.Organizations.Any(it => it.private_id == privateId))
                        {
                            privateId = Guid.NewGuid();
                        }

                        Organizations newOrganiz = new Organizations
                        {
                            private_id = privateId,
                            public_id = info.publicId,
                            org_name_part = info.name,
                            kb_of_credit = 0,
                            owner = owner
                        };
                        newOrganiz.members.Add(owner);
                        db.Organizations.Add(newOrganiz);
                        Transaction.saveTransaction(db, info.transactionId, Transaction.TABLE_NAME.ORGANIZATIONS, newOrganiz.private_id.ToString(), owner);
                        db.SaveChanges();
                        output.data = new { publicId = newOrganiz.public_id, name = newOrganiz.org_name_part };
                    }
                    else
                    {
                        output.success = false;
                        output.msgError = "Change the publicId";
                    }
                }
                else
                {
                    output.success = false;
                    output.msgError = "You a have already organization";
                }
            }

            rez.msgForOwner = output;
            return rez;
        }
    }
}
