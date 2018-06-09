using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using RAIDAChatNode.DTO.Configuration;
using RAIDAChatNode.Utils;
using RAIDAChatNode.Extensions;

namespace RAIDAChatNode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoadConfiguration.Load();
            SystemClock.GetInstance();
            //new SystemClock(); --Singleton, autorun
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.ConfigureEndpoints();
                    new SyncData().Sync();
                })
                .UseUrls(MainConfig.Connection.ToString())
                .Build();
    }
}
