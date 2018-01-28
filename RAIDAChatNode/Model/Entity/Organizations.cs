using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RAIDAChatNode.Model.Entity
{
    public class Organizations
    {
        [Key]
        public Guid private_id { get; set; }
        public Guid public_id { get; set; }
        public string org_name_part { get; set; }
        public int kb_of_credit { get; set; }
        public Members owner { get; set; }
        public ICollection<Members> members { get; set; }
        public ICollection<Groups> groups { get; set; }
        public Organizations()
        {
            members = new List<Members>();
            groups = new List<Groups>();
        }
    }
}
