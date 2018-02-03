using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAIDAChatNode.DTO
{
    public class SetDialogPrivateInfo
    {
        public Guid publicId { get; set; }
        public Boolean privated { get; set; }
    }
}
