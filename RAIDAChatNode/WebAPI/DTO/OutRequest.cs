using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace RAIDAChatNode.WebAPI.DTO
{
    public class OneTableHash
    {
        [Required] 
        public RSAParameters PublicKey { get; set; }
        [Required]
        public List<string> ActualMembers { get; set; }
        [Required]
        public List<string> ActualGroups { get; set; }
        [Required]
        public List<string> ActualMinG { get; set; }

        public OneTableHash()
        {
            ActualMembers = new List<string>();
            ActualGroups = new List<string>();
            ActualMinG = new List<string>();
        }
    }
}