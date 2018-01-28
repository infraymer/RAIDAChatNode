using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RAIDAChatNode.Model.Entity
{
    public class Members
    {
        [Key]
        public Guid private_id { get; set; }
        public string login { get; set; }
        public string pass { get; set; }   
        public string nick_name { get; set; }
        public long last_use { get; set; }
        public string description_fragment { get; set; }
        public byte[] photo_fragment { get; set; }
        public int kb_bandwidth_used { get; set; }
        public string away_busy_ready { get; set; }

        public virtual Organizations organization { get; set; }
        public virtual ICollection<MemberInGroup> MemberInGroup { get; set; }

        public Members()
        {
            MemberInGroup = new List<MemberInGroup>();
        }

    }
}
