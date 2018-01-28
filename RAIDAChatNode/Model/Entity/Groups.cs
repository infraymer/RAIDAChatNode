using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RAIDAChatNode.Model.Entity
{
    public class Groups
    {
        [Key]
        public Guid group_id { get; set; }
        public string group_name_part { get; set; }
        public string photo_fragment { get; set; }

        public string allow_or_deny { get; set; }
        public bool one_to_one { get; set; }

        public Members owner { get; set; }
        public virtual Organizations organization { get; set; }
        public virtual ICollection<MemberInGroup> MemberInGroup { get; set; }


        public Groups()
        {
            MemberInGroup = new List<MemberInGroup>();
        }

    }
}
