using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using RAIDAChatNode.WebAPI.DTO;

namespace RAIDAChatNode.Utils
{
    public class CryptoUtils
    {
        public static (RSAParameters privateKey, RSAParameters publicKey) GenerateRSAKeys()
        {
            using (RSA Rsa = RSA.Create())
            {
                return(Rsa.ExportParameters(true), Rsa.ExportParameters(false)); 
            }
        }
 
        public static byte[] EncryptKeyRSA(RSAParameters PublicRSAkey, object AESKey)
        {
            byte[] encrypt;
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(PublicRSAkey);
                byte[] symKey = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(AESKey));
                encrypt = rsa.Encrypt(symKey, RSAEncryptionPadding.OaepSHA512);
            }
            return encrypt;
        }

        public static SecretAESKey DecryptSecretAESKey(RSAParameters privateKey, string encryptedKey)
        {
            byte[] bKey = Convert.FromBase64String(encryptedKey);
            using (RSA Rsa = RSA.Create())
            {
                Rsa.ImportParameters(privateKey);
                String json = Encoding.UTF8.GetString(Rsa.Decrypt(bKey, RSAEncryptionPadding.OaepSHA512));
                SecretAESKey obj = JsonConvert.DeserializeObject<SecretAESKey>(json); 
                return obj;
            }
        }
        
        public static (object key, byte[] code) EncryptDataAES(object msg)
        {
            byte[] text = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
            
            Aes aes = Aes.Create();
            object PrivateKey = new {aes.Key, aes.IV };

            byte[] code;
            using (ICryptoTransform encrypt = aes.CreateEncryptor())
            {
                code = encrypt.TransformFinalBlock(text, 0, text.Length);
            }

            return (PrivateKey, code);
        }
        
        public static T DecryptDataAES<T>(SecretAESKey key, string encryptedData)
        {
            byte[] text = Convert.FromBase64String(encryptedData);
            Aes aes = Aes.Create();
            byte[] origin;
            using (ICryptoTransform decryptor = aes.CreateDecryptor(key.Key, key.IV))
            {
                origin = decryptor.TransformFinalBlock(text, 0, text.Length);
            }
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(origin));
        }
        
        
        public class SecretAESKey
        {
            public byte[] Key { get; set; }
            public byte[] IV { get; set; }
        }
    }
}