using RAIDAChatNode.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Internal;
using Newtonsoft.Json;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;
using SystemClock = RAIDAChatNode.Utils.SystemClock;

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
                    Members user = db.Members.FirstOrDefault(it => it.login.Equals(info.login.Trim(), StringComparison.CurrentCultureIgnoreCase) && Argon2.Verify(it.pass, info.password, null));
                    if(user != null){
                        output.auth = true;
                        output.login = user.login;
                        output.nickName = user.nick_name;
                        output.password = info.password;

                        user.online = true;
                        user.last_use = SystemClock.CurrentTime; 
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
