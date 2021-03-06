﻿using System;
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
        public string photo_fragment { get; set; }
        public int kb_bandwidth_used { get; set; }
        public bool online { get; set; }

        public virtual Organizations organization { get; set; }
        public virtual ICollection<MemberInGroup> MemberInGroup { get; set; }
        public virtual ICollection<Transactions> Transactions { get; set; }

        public Members()
        {
            MemberInGroup = new List<MemberInGroup>();
            Transactions = new List<Transactions>();
        }

    }
}
