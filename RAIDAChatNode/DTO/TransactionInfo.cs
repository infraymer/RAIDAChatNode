using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAIDAChatNode.DTO
{
    public class TransactionInfo : ITransactionInfo
    {
        public Guid transactionId { get; set; }
    }
}
