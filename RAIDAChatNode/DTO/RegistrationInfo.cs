using System;

namespace RAIDAChatNode.DTO
{
    public class RegistrationInfo : AuthInfo, ITransactionInfo
    {
        public string nickName { get; set; }
        public Guid transactionId { get; set;}
    }
}