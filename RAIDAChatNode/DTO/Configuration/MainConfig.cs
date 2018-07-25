using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RAIDAChatNode.Utils;

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
        [Required, ValidateObject]
        public Connection Connection { get; set; }

        [JsonProperty(PropertyName = "DataBase")]
        [Required, ValidateObject]
        public DataBase DB { get; private set; }

        [Range(0, 3600,
            ErrorMessage = "Configurations is not load: TransactionRollback is not valid [Range(0-3600)]")]
        public int TransactionRollback { get;  set; }
        
        [Range(1, int.MaxValue,
            ErrorMessage = "Configurations is not load: LimitMsgForDialog is not valid [Range(1-int.MaxValue)]")]
        public int LimitMsgForDialog { get;  set; }
        
        [Range(60, 86400,
            ErrorMessage = "Configurations is not load: CleanUpTimer is not valid [Range(60-86400)]")]
        public int CleanUpTimer { get;  set; }
        
        [Range(1800, 86400,
            ErrorMessage = "Configurations is not load: SyncWorldTime is not valid [Range(1800-86400)]")]
        public int SyncWorldTime { get;  set; }
        
        [MinLength(1, 
            ErrorMessage = "Configurations is not load: Count of trusted server must be > 0")]
        public List<string> TrustedServers { get;  set; }

        public SerializeMainConfig()
        {
            TransactionRollback = 60;
            LimitMsgForDialog = 100;
            CleanUpTimer = 3600;
            SyncWorldTime = 86400;
            TrustedServers = new List<string>();
            DB = new DataBase();
        }
    }
    
    
}