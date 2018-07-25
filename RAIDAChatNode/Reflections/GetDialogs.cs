using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;
using RAIDAChatNode.Utils;
// ReSharper disable All

namespace RAIDAChatNode.Reflections
{
    public class GetDialogs : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin, Guid actId)
        {
            #region Тестовые данные
            /* { 
                    "execFun": "GetDialogs", 
                    "data": {
                        "msgCount": "2"
                    }
                }
            */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("getMsg", actId, true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            GetDialogsInfo info = DeserializeObject.ParseJSON<GetDialogsInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {
                Members owner = db.Members.First(it => it.login.Equals(myLogin));

                // ReSharper disable once SuggestVarOrType_Elsewhere
                List<OutGetMsgInfo> list = new List<OutGetMsgInfo>();
                //I'am consist in this dialogs
                List<MemberInGroup> mg = db.MemberInGroup.Include(g => g.group)
                    .ThenInclude(s => s.Shares).ThenInclude(own => own.owner)
                    .Where(it => it.member.login.Equals(myLogin))
                    .ToList();

                mg.ForEach(it => {
                    string groupName = it.group.group_name_part;
                    if (it.group.one_to_one)
                    {
                        groupName = db.MemberInGroup.Include(m => m.member).FirstOrDefault(mig => mig.group == it.group && mig.member != owner)?.member.nick_name;
                    }
                    OutGetMsgInfo group = new OutGetMsgInfo(it.group.group_id, groupName, it.group.one_to_one, it.group.privated);
                    //Add all message in dialog
                    it.group.Shares.OrderBy(s => s.sending_date)
                        .Take(info.msgCount)
                        .ToList()
                        .ForEach(msg => group.messages.Add(new OneMessageInfo(msg.id, Encoding.Unicode.GetString(msg.file_data), msg.current_fragment, msg.total_fragment, msg.sending_date, msg.owner.nick_name, msg.owner.login)));

                    //Add users where consist in dialog 
                    db.MemberInGroup.Include(m => m.member).Where(mig => mig.groupId.Equals(it.groupId))
                        .ForEachAsync(memb => group.members.Add(memb.member.login));
                    
                    list.Add(group);
                });

                output.data = list;
            }

            rez.msgForOwner = output;
            return rez;
        }
    }
}
