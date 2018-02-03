using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAIDAChatNode.DTO
{
    public class OrganizationCreateInfo : ITransactionInfo
    {
        public Guid publicId { get; set; }
        public string name { get; set; }
        public Guid transactionId { get; set; }
    }
}
