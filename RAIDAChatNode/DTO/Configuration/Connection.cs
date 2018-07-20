using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RAIDAChatNode.Utils;

namespace RAIDAChatNode.DTO.Configuration
{
    public class Connection
    {
        [Required( 
            ErrorMessage = "Configurations is not load: Connection addres is empty")]
        /*[RegularExpression(@"^\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}\b$", 
            ErrorMessage = "Configurations is not load: Connection IP is not valid")]*/
        public string Addr { get; set; }
        
        [Range(1, 65535,
            ErrorMessage = "Configurations is not load: Connection Port is not valid [Range(1-65535)]")]
        public int Port { get; set; }
        
        public SSL SSL { get; set; }

        public Connection()
        {
            Port = 49001;
            //HTTPS = false;
        }

        public override string ToString()
        {
            string https = SSL != null ? "https" : "http"; 
            return $"{https}://{Addr}:{Port}";
        }
    }
}