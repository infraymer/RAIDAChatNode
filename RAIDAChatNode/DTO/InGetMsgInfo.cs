using System;

namespace RAIDAChatNode.DTO
{ 
    public class InGetMsgInfo
    {
        public bool getAll { get; set; }
        public bool onGroup { get; set; }
        public Guid onlyId { get; set; }

    }

    public class OutGetMsgInfo
    {
        public Guid guidMsg { get; set; }
        public string textMsg { get; set; }
        public Guid sender { get; set; }
        public string group { get; set; }

        public Guid recipient { get; set; }

        public int curFrg { get; set; }
        public int totalFrg { get; set; }
        public long sendTime { get; set; }

        public string senderName { get; set; }
        public string groupName { get; set; }
        public OutGetMsgInfo() {}

        public OutGetMsgInfo(Guid guidMsg, string textMsg, Guid sender, string group, Guid recipient, long _time, int curFrg, int totalFrg, string senderName, string groupName)
        {
            this.guidMsg = guidMsg;
            this.textMsg = textMsg;
            this.sender = sender;
            this.group = group;
            this.recipient = recipient;
            this.sendTime = _time;
            this.curFrg = curFrg;
            this.totalFrg = totalFrg;
            this.senderName = senderName;
            this.groupName = groupName;
        }
    }

}