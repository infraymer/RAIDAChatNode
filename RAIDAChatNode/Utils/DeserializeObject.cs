using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using RAIDAChatNode.DTO;
using Newtonsoft.Json;

namespace RAIDAChatNode.Utils
{
    public static class DeserializeObject
    {
        public static T ParseJSON<T> (object json, OutputSocketMessage outp, out OutputSocketMessageWithUsers rez)
        {
            rez = new OutputSocketMessageWithUsers();
            T sucObj = default(T);
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                sucObj = JsonConvert.DeserializeObject<T>(json.ToString(), settings);
            }
            catch
            {
                outp.success = false;
                outp.msgError = "Invalid sending data";
                rez.msgForOwner = outp;
            }
            return sucObj;
        }

       
        public static void IsValid(object model)
        {
            var validCOntext = new ValidationContext(model, null, null);
            Validator.ValidateObject(model, validCOntext, true);
        }
        
    }
}
