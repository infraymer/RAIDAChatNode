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
    public class OrganizationAddMember : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin)
        {

            #region Тестовые данные
            /* { 
                    "execFun": "OrganizationAddMember", 
                    "data": {
                        "login": "shREkUp", 
                        "password": "qwerty", 
                        "nickName": "Serj",
                        "transactionId": "80f7efc032cd4a2c97f89fca11ad3701"
                    }
                }
            */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("OrganizationAddMember", true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            using (var db = new RaidaContext())
            {
                Members owner = db.Members.Include(o => o.organization).First(it => it.login.Equals(myLogin));
                if(owner.organization != null && owner.organization.owner == owner)
                {
                    Registration registration = new Registration();
                    rez = registration.Execute(val, myLogin);
                    rez.msgForOwner.callFunction = "OrganizationAddMember";
                    if (rez.msgForOwner.success)
                    {
                        try
                        {
                            RegistrationInfo info = DeserializeObject.ParseJSON<RegistrationInfo>(val, output, out rez);
                            Organizations organization = db.Organizations.Include(m => m.members).First(it => it == owner.organization);
                            Members newMember = db.Members.First(it => it.login.Equals(info.login.Trim(), StringComparison.CurrentCultureIgnoreCase));
                            newMember.organization = organization;
                            organization.members.Add(newMember);
                        
                            db.SaveChanges();
                            output.data = new { newMember.login, nickName = newMember.nick_name };
                            rez.msgForOwner = output;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        
                                
                    }
                }
                else
                {
                    output.success = false;
                    output.msgError = "You a not consist in organization or a not organization owner";
                    rez.msgForOwner = output;

                }
            }
            
            return rez;
        }
    }
}
