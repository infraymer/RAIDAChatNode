using System;
using Newtonsoft.Json;

namespace RAIDAChatNode.WebAPI.DTO
{
    public class SyncGroupInfo
    {
        [JsonProperty(PropertyName = "group_name_part")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "group_id")]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "one_to_one")]
        public bool PeerToPeer { get; set; }
        
        [JsonProperty(PropertyName = "photo_fragment")]
        public string Photo { get; set; }
        
        [JsonProperty(PropertyName = "privated")]
        public bool Privated { get; set; }
        
        [JsonProperty(PropertyName = "owner")]
        public string Owner { get; set; }
    }
}