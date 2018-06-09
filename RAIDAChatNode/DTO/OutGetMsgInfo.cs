using System;
using System.Collections.Generic;

namespace RAIDAChatNode.DTO
{ 
    public class OutGetMsgInfo
    {
        public Guid dialogId { get; set; }        
        public string groupName { get; set; }
        public bool oneToOne { get; set; }
        public bool privated { get; set; }
        public List<string> members { get; set; }
        public List<OneMessageInfo> messages { get; set; }

        public OutGetMsgInfo()
        {
            messages = new List<OneMessageInfo>();
            members = new List<string>();
        }
        public OutGetMsgInfo(Guid dialogId, string groupName, bool oneToOne, bool privated):this()
        {
            this.dialogId = dialogId;
            this.groupName = groupName;
            this.oneToOne = oneToOne;
            this.privated = privated;
        }

        public OutGetMsgInfo(Guid dialogId, string groupName, bool oneToOne, bool privated, List<OneMessageInfo> msgs) : this(dialogId, groupName, oneToOne, privated)
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
        public string senderLogin { get; set; }

        public OneMessageInfo(){}

        public OneMessageInfo(Guid guidMsg, string textMsg, int curFrg, int totalFrg, long sendTime, string senderName, string senderLogin)
        {
            this.guidMsg = guidMsg;
            this.textMsg = textMsg;
            this.curFrg = curFrg;
            this.totalFrg = totalFrg;
            this.sendTime = sendTime;
            this.senderName = senderName;
            this.senderLogin = senderLogin;
        }
    }

}