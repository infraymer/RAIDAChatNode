using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RAIDAChatNode.DTO.Configuration
{
    public class Connection
    {
        [Required]
        [RegularExpression(@"^\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}\b$", 
            ErrorMessage = "Configurations is not load: Connection IP is not valid")]
        public string IP { get; set; }
        
        [Range(1, 65535,
            ErrorMessage = "Configurations is not load: Connection Port is not valid [Range(1-65535)]")]
        public int Port { get; set; }
        public bool HTTPS { get; set; }

        public Connection()
        {
            Port = 49001;
            HTTPS = false;
        }

        public override string ToString()
        {
            string https = HTTPS ? "https" : "http"; 
            return $"{https}://{IP}:{Port}";
        }
    }
}