using System;
using System.Text;

namespace RAIDAChatNode.DTO.Configuration
{
    public class Connection
    {
        public string IP { get; set; }
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