using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using RAIDAChatNode.DTO.Configuration;
using RAIDAChatNode.Utils;

namespace RAIDAChatNode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoadConfiguration.Load();
            //new SystemClock();
            new SyncData().Sync();
            
            BuildWebHost(args).Run();
           
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(MainConfig.Connection.ToString())
                .Build();
    }
}
