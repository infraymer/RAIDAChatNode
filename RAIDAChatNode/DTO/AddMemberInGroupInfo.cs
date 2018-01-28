using System;

namespace RAIDAChatNode.DTO
{
    public class AddMemberInGroupInfo: ITransactionInfo
    {
        public String memberLogin { get; set; }
        public Guid groupId { get; set; }
        public Guid transactionId { get; set; }
    }
}