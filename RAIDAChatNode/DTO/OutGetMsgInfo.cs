using System;
using System.Collections.Generic;

namespace RAIDAChatNode.DTO
{ 
    public class OutGetMsgInfo
    {
        public Guid dialogId { get; set; }        
        public string groupName { get; set; }
        public List<OneMessageInfo> messages { get; set; }

        public OutGetMsgInfo()
        {
            messages = new List<OneMessageInfo>();
        }
        public OutGetMsgInfo(Guid dialogId, string groupName):this()
        {
            this.dialogId = dialogId;
            this.groupName = groupName;
        }

        public OutGetMsgInfo(Guid dialogId, string groupName, List<OneMessageInfo> msgs) : this(dialogId, groupName)
        {
            messages = msgs;
        }
    }


    public class OneMessageInfo
    {
        public Guid guidMsg { get; set; }
        public string textMsg { get; set; }
        public int curFrg { get; set; }
        public int totalFrg { get; set; }
        public long sendTime { get; set; }
        public string senderName { get; set; }

        public OneMessageInfo(){}

        public OneMessageInfo(Guid guidMsg, string textMsg, int curFrg, int totalFrg, long sendTime, string senderName)
        {
            this.guidMsg = guidMsg;
            this.textMsg = textMsg;
            this.curFrg = curFrg;
            this.totalFrg = totalFrg;
            this.sendTime = sendTime;
            this.senderName = senderName;
        }
    }

}