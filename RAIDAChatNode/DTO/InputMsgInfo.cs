using System;

namespace RAIDAChatNode.DTO
{ 
    public class InputMsgInfo: ITransactionInfo
    {
        public Guid dialogId { get; set; }
        public string textMsg { get; set; }
        public long deathDate { get; set; }

        public int curFrg { get; set; }
        public int totalFrg { get; set; }

        public Guid msgId { get; set; }
        public Guid transactionId { get; set; }
    }
}