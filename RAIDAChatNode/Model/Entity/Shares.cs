using System;
using System.ComponentModel.DataAnnotations;

namespace RAIDAChatNode.Model.Entity
{
    public class Shares
    {
        [Key]
        public Guid id { get; set; }
        public long death_date { get; set; }
        public int kb_size { get; set; }
        public string file_extention { get; set; }
        public int total_fragment { get; set; }
        public int current_fragment { get; set; }
        public long sending_date { get; set; }
        public byte[] file_data { get; set; }

        public Members owner { get; set; }
        public Groups to_group { get; set; }
        public virtual Organizations organization { get; set; }

        public Shares()
        {
        }

    }
}
