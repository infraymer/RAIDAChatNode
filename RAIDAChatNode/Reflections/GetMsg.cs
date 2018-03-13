using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Utils;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace RAIDAChatNode.Reflections
{
    public class GetMsg : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin)
        {

            #region Тестовые данные
            /* { 
                    "execFun": "GetMsg", 
                    "data": {
                        "dialogId": "788FEFAD0ED24436AD73D968685110E8",
                        "msgCount": "2",
                        "offset": "0"
                    }
                }
            */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("getMsg", true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            GetMsgInfo info = DeserializeObject.ParseJSON<GetMsgInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {
                Members owner = db.Members.First(it => it.login.Equals(myLogin));

                if(db.MemberInGroup.Include(g => g.group).Any(it => it.member.login.Equals(myLogin) && it.groupId == info.dialogId))
                {
                    MemberInGroup mg = db.MemberInGroup.Include(g => g.group).ThenInclude(s => s.Shares).Where(it => it.member.login.Equals(myLogin) && it.groupId == info.dialogId).First();
                    
                    string groupName = mg.group.group_name_part;
                    if (mg.group.one_to_one)
                    {
                        groupName = db.MemberInGroup.Include(m => m.member).FirstOrDefault(mig => mig.group == mg.group && mig.member != owner)?.member.nick_name;
                    }
                    OutGetMsgInfo groupMsg = new OutGetMsgInfo(mg.group.group_id, groupName, mg.group.one_to_one);
                    mg.group.Shares.OrderBy(s => s.sending_date)
                        .Skip(info.offset)
                        .Take(info.msgCount)
                        .ToList()
                        .ForEach(msg => groupMsg.messages.Add(new OneMessageInfo(msg.id, Encoding.Unicode.GetString(msg.file_data), msg.current_fragment, msg.total_fragment, msg.sending_date, msg.owner.nick_name)));

                    output.data = groupMsg;
                }
                else
                {
                    output.success = false;
                    output.msgError = "Dialog is not found";
                }
            }

            rez.msgForOwner = output;
            return rez;
        }
    }
}
