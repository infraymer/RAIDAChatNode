using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAIDAChatNode.DTO
{
    public class GetMsgInfo
    {
        public Guid dialogId { get; set; }
        public int msgCount { get; set; }
        public int offset { get; set; }
    }
}
