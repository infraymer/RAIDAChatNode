using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RAIDAChatNode.WebAPI.DTO
{
    public class SyncMemberInfo : ICollcetionObject
    {
        [JsonProperty(PropertyName = "description_fragment")]
        public string Descript { get; set; }
        
        [JsonProperty(PropertyName = "last_use")]
        public long LastUse { get; set; }
        
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }
        
        [JsonProperty(PropertyName = "pass")]
        public string Password { get; set; }
        
        [JsonProperty(PropertyName = "nick_name")]
        public string NickName { get; set; }

        [JsonProperty(PropertyName = "online")]
        public bool Online { get; set; }
        
        [JsonProperty(PropertyName = "photo_fragment")]
        public byte[] Photo { get; set; }
        
        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }
    }
}