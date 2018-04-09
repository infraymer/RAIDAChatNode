using System;

namespace RAIDAChatNode.WebAPI.DTO
{
    public class SyncMembInGroupInfo : ICollcetionObject
    {
        public Guid GroupId { get; set; }
        public string Login { get; set; }
        public string Hash { get; set; }
    }
}