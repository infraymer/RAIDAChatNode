using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Newtonsoft.Json.Serialization;

namespace RAIDAChatNode.WebAPI.DTO
{
    public class OneTableHash
    {
        [Required]
        public string Name { get; set; }

        [Required] 
        public RSAParameters PublicKey { get; set; }
        
        [Required]
        public List<string> Rows { get; set; }

        public OneTableHash()
        {
            Rows = new List<string>();
        }
    }

    public class InputSyncData
    {
        public string Key { get; set; }
        public string Code { get; set; }
    }

    public class SecretAESKey
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
    }

    public class DataInfo<T>
    {
        public List<T> NewRows { get; set; }
        public List<string> Actual { get; set; }

        public DataInfo()
        {
            NewRows = new List<T>();
            Actual = new List<string>();
        }
    }
}