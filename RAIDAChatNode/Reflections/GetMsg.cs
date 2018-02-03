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
            OutputSocketMessage output = new OutputSocketMessage("getMsg", true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            using (var db = new RaidaContext())
            {
                Members owner = db.Members.First(it => it.login.Equals(myLogin));

                List<OutGetMsgInfo> list = new List<OutGetMsgInfo>();
                List<MemberInGroup> mg = db.MemberInGroup.Include(g => g.group).ThenInclude(s => s.Shares).Where(it => it.member.login.Equals(myLogin)).ToList();

                mg.ForEach(it => {
                    string groupName = it.group.group_name_part;
                    if (it.group.one_to_one)
                    {
                        try
                        {
                            groupName = db.MemberInGroup.Include(m => m.member).FirstOrDefault(mig => mig.group == it.group && mig.member != owner)?.member.nick_name;
                        }catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    OutGetMsgInfo group = new OutGetMsgInfo(it.group.group_id, groupName);
                    it.group.Shares.OrderBy(s => s.sending_date)
                        .ToList()
                        .ForEach(msg => group.messages.Add(new OneMessageInfo(msg.id, Encoding.Unicode.GetString(msg.file_data), msg.current_fragment, msg.total_fragment, msg.sending_date, msg.owner.nick_name)));
                    list.Add(group);
                });

                output.data = list;
            }

            rez.msgForOwner = output;
            return rez;
        }
    }
}
