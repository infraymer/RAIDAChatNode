using System.Collections.Generic;
using Newtonsoft.Json;

namespace RAIDAChatNode.DTO.Configuration
{
    public class MainConfig
    {
        public static Connection Connection { get; private set; }
        public static DataBase DB { get; private set; }

        public static int TransactionRollback { get; private set; }
        public static int LimitMsgForDialog { get; private set; }
        public static int CleanUpTimer { get; private set; }
        public static int SyncWorldTime { get; private set; }
        public static List<string> TrustedServers { get; private set; }

        public static void Mapping(SerializeMainConfig conf)
        {
            Connection = conf.Connection;
            DB = conf.DB;
            TransactionRollback = conf.TransactionRollback;
            LimitMsgForDialog = conf.LimitMsgForDialog;
            CleanUpTimer = conf.CleanUpTimer;
            SyncWorldTime = conf.SyncWorldTime;
            TrustedServers = conf.TrustedServers;
        }
    }
    
    
    public class SerializeMainConfig
    {
        public Connection Connection { get; set; }

        [JsonProperty(PropertyName = "DataBase")]
        public DataBase DB { get; private set; }

        public int TransactionRollback { get;  set; }
        public int LimitMsgForDialog { get;  set; }
        public int CleanUpTimer { get;  set; }
        public int SyncWorldTime { get;  set; }
        public List<string> TrustedServers { get;  set; }

        public SerializeMainConfig()
        {
            TransactionRollback = 60;
            LimitMsgForDialog = 100;
            CleanUpTimer = 3600;
            SyncWorldTime = 86400;
            TrustedServers = new List<string>();
        }
    }
    
    
}