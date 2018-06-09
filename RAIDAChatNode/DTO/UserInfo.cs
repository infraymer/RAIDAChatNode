using System;

namespace RAIDAChatNode.DTO
{
    public class UserInfo : IEquatable<UserInfo>
    {
        public string login { get; set; }
        public string nickName { get; set; }
        public string photo { get; set; }
        public bool online { get; set; }

        public UserInfo()
        {
        }

        public UserInfo(string login, string nickName, string photo, bool online)
        {
            this.login = login;
            this.nickName = nickName;
            this.photo = photo;
            this.online = online;
        }

        public bool Equals(UserInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(login, other.login) && string.Equals(nickName, other.nickName) && string.Equals(photo, other.photo) && online == other.online;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (login != null ? login.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (nickName != null ? nickName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (photo != null ? photo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ online.GetHashCode();
                return hashCode;
            }
        }
    }
}