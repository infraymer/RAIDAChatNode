using System;
using System.Collections.Generic;

namespace RAIDAChatNode.DTO
{
    public class OutputSocketMessage
    {
        public string callFunction { get; set; }
        public Guid actId { get; set; }
        public bool success { get; set; }
        public string msgError { get; set; }
        public Object data { get; set; }

        public OutputSocketMessage(string callFunction, Guid actId, bool success, string msgError, object data)
        {
            this.callFunction = callFunction;
            this.actId = actId;
            this.success = success;
            this.msgError = msgError;
            this.data = data;
        }
    }

    public class OutputSocketMessageWithUsers
    { 
        public OutputSocketMessageWithUsers()
        {
            forUserLogin = new List<String>();
        }
        
        public OutputSocketMessage msgForOwner { get; set; }
     
        public object msgForOther { get; set; }
        public List<String> forUserLogin { get; set; }
    }
}