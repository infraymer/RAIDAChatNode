using RAIDAChatNode.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Isopoh.Cryptography.Argon2;
using Newtonsoft.Json;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;

namespace RAIDAChatNode.Reflections
{
    public class Authorization
    {

        /* { "execFun": "authorization", 
                "data": {
                    "login": "shRek",
                    "password": "qwerty" 
                } 
            }
          */


         //Замутить обновление статуса онлайн-офлайн
        public AuthSocketInfo Execute(object data)
        {
            AuthSocketInfo output = new AuthSocketInfo();

            AuthInfo info;
            try
            {
                info = JsonConvert.DeserializeObject<AuthInfo>(data.ToString());

                using (var db = new RaidaContext())
                {

                    //if (db.Members.Any(it => it.login.Equals(info.login.Trim(), StringComparison.CurrentCultureIgnoreCase) && it.pass.Equals(info.password)))
                    if (db.Members.Any(it => it.login.Equals(info.login.Trim(), StringComparison.CurrentCultureIgnoreCase) && Argon2.Verify(it.pass, info.password, null)))
                    {
                        //Members user = db.Members.First(it => it.login.Equals(info.login.Trim(), StringComparison.CurrentCultureIgnoreCase) && it.pass.Equals(info.password));
                        Members user = db.Members.First(it => it.login.Equals(info.login.Trim(), StringComparison.CurrentCultureIgnoreCase) && Argon2.Verify(it.pass, info.password, null));

                        output.auth = true;
                        output.login = user.login;
                        output.nickName = user.nick_name;
                        output.password = info.password;

                        user.online = true;
                        user.last_use = DateTimeOffset.Now.ToUnixTimeSeconds();
                        db.SaveChanges();
                    }
                    else
                    {
                        output.auth = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                output.auth = false;
            }
            return output;
        }
    }
}
