using NetSocket.Sockets;
using System;
using Isopoh.Cryptography.Argon2;
// ReSharper disable All

namespace RAIDAChatNode.DTO
{

    public class AuthInfo
    {
        public String login { get; set; }
        public String password { get; set; }
    }

    public class AuthSocketInfo : AuthInfo
    {
        public bool auth { get; set; }
        public String nickName { get; set; }
        public IClient client { get; set; }
    }
}