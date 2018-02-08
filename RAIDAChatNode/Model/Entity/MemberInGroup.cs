using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RAIDAChatNode.Model.Entity
{
    public class MemberInGroup
    {
        [Key]
        public int Id { get; set; }
        public Guid groupId { get; set; }
        public Groups group { get; set; }

        public Guid memberId { get; set; }
        public Members member { get; set; }
    }
}
