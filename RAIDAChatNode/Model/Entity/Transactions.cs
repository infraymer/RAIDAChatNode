using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAIDAChatNode.Model.Entity
{
    public class Transactions
    {
        public int Id { get; set; }
        public Guid transactionId { get; set; }
        public long rollbackTime { get; set; }
        public string tableName { get; set; }
        public string tableRowId { get; set; }
        public Members ownerId { get; set; }

        public Transactions()
        {
        }
    }
}
