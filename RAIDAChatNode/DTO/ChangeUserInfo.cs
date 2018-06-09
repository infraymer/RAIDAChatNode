using System;

namespace RAIDAChatNode.DTO
{
    public class ChangeUserInfo 
    {
        public string Photo { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public bool ChangePass { get; set; }
        public string OldPass { get; set; }
        public string NewPass { get; set; }
    }
}