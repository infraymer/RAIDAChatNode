using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Utils;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;

using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RAIDAChatNode.Reflections
{
    public class SendMsg : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin, Guid actId)
        {
            #region Тестовые данные
            /*
                        {
                            "execFun": "sendMsg",
                            "data": {
                                "dialogId": "788FEFAD0ED24436AD73D968685110E8",     
                                "textMsg": "test message for one user",
                                "guidMsg": "91D8333FA55B40AFB46CA63E214C93C8",
                                "curFrg":"1",
                                "totalFrg":"2",
                                "transactionId": "80f7efc032dd4a1c97f69fca51ad1100",
                                "deathDate": "2511450497620"
                            }
                        }
                        */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("sendMsg", actId, true, "", new { });
            OutputSocketMessage outputOther = new OutputSocketMessage("sendMsg", Guid.Empty, true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            InputMsgInfo info = DeserializeObject.ParseJSON<InputMsgInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {
                Members owner = db.Members.First(it => it.login.Equals(myLogin));

                if(db.Groups.Any(it => it.group_id == info.dialogId && it.MemberInGroup.Any(mg => mg.group == it && mg.member == owner))) //Check created group and member in this group
                {
                    if (!db.Shares.Any(it => it.id == info.msgId)) //Check exist message with Id
                    {
                        Groups gr = db.Groups.Include(s => s.Shares).First(it => it.group_id == info.dialogId);
                        Console.WriteLine(SystemClock.GetInstance().CurrentTime.ToString());
                        Shares msg = new Shares
                        {
                            id = info.msgId,
                            owner = owner,
                            current_fragment = info.curFrg,
                            total_fragment = info.totalFrg,
                            file_data = Encoding.Unicode.GetBytes(info.textMsg),
                            file_extention = "none",
                            kb_size = 0,
                            sending_date = SystemClock.GetInstance().CurrentTime, // DateTimeOffset.Now.ToUnixTimeSeconds(),
                            death_date =
                                info.deathDate > 0
                                    ? DateConvert.validateTimestamp(SystemClock.GetInstance().CurrentTime + info.deathDate)
                                    : DateTimeOffset.Now.AddYears(2000).ToUnixTimeSeconds(),
                            to_group = gr
                        };
                        gr.Shares.Add(msg);
                        db.Shares.Add(msg);

                        Transaction.saveTransaction(db, info.transactionId, Transaction.TABLE_NAME.SHARES,
                            msg.id.ToString(), owner);

                        db.SaveChanges();

                        List<MemberInGroup> membersInGroup = db.MemberInGroup.Include(m => m.member)
                            .Where(it => it.group == gr && it.member != owner).ToList();
                        membersInGroup.ForEach(it => rez.forUserLogin.Add(it.member.login));

                        string groupNameForOwner = gr.group_name_part,
                            groupNameForOther = gr.group_name_part;

                        if (gr.one_to_one)
                        {
                            groupNameForOwner = membersInGroup.First(it => it.member != owner).member.nick_name;
                            groupNameForOther = owner.nick_name;
                        }

                        OneMessageInfo newMsg = new OneMessageInfo(msg.id, info.textMsg, msg.current_fragment,
                            msg.total_fragment, msg.sending_date, owner.nick_name, msg.owner.login);

                        output.data = new OutGetMsgInfo(gr.group_id, groupNameForOwner, gr.one_to_one, gr.privated,
                            new List<OneMessageInfo>() {newMsg});
                        outputOther.data = new OutGetMsgInfo(gr.group_id, groupNameForOther, gr.one_to_one, gr.privated,
                            new List<OneMessageInfo>() {newMsg});
                        
                        Transaction.removeMessageAboveLimit(gr);
                    }
                    else
                    {
                        output.success = false;
                        output.msgError = "Message with this ID is exist";
                        output.data = new { id = info.msgId };
                        rez.msgForOwner = output;
                        return rez; 
                    }
                }
                else
                {
                    output.success = false;
                    output.msgError = "Dialog is not found";
                    rez.msgForOwner = output;
                    return rez;
                }

            }

            rez.msgForOwner = output;
            rez.msgForOther = outputOther;
            return rez;
        }
    }
}
