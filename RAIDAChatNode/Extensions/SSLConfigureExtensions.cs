using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RAIDAChatNode.DTO.Configuration;
using RAIDAChatNode.Utils;

namespace RAIDAChatNode.Extensions
{
    public static class SSLConfigureExtensions
    {
        public static void ConfigureEndpoints(this KestrelServerOptions options)
        {
            if (MainConfig.Connection.SSL != null)
            {
                try
                {
                    DeserializeObject.IsValid(MainConfig.Connection.SSL);
                }
                catch (Exception ex)
                {
                    LoadConfiguration.CloseApp(ex.Message);
                
                }
                
                var environment = options.ApplicationServices.GetRequiredService<IHostingEnvironment>();
                
                options.Listen(IPAddress.Parse(MainConfig.Connection.SSL.IP), MainConfig.Connection.Port,
                    listenOptions =>
                    {
                        var certificate = LoadCertificate(environment);
                        listenOptions.UseHttps(certificate);
                    }); 
            }
        }
        
        private static X509Certificate2 LoadCertificate(IHostingEnvironment environment)
        {
            if (MainConfig.Connection.SSL.SerialNumb != null)
            {
                using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    var SSLMatch = new Regex("[^a-fA-F0-9]");
                    store.Open(OpenFlags.ReadOnly);
                    var certificate = store.Certificates.Find(
                        X509FindType.FindBySerialNumber,
                        SSLMatch.Replace(MainConfig.Connection.SSL.SerialNumb, string.Empty).ToUpper(),
                        validOnly: true);


                    if (certificate.Count == 0)
                    {
                        LoadConfiguration.CloseApp($"Certificate not found for serial number {MainConfig.Connection.SSL.SerialNumb}.");
                    }

                    return certificate[0];
                } 
            }
    
            if (MainConfig.Connection.SSL?.PathFile != null && MainConfig.Connection.SSL?.Password != null)
            {
                try
                {
                    return new X509Certificate2(MainConfig.Connection.SSL.PathFile, MainConfig.Connection.SSL.Password);
                }
                catch (Exception ex)
                {
                    LoadConfiguration.CloseApp($"Error read certificate: {ex.Message}");
                }
                
            }
    
            LoadConfiguration.CloseApp("No valid certificate configuration found for the current endpoint.");
            throw new InvalidOperationException("No valid certificate configuration found for the current endpoint.");
        }
    }
}
