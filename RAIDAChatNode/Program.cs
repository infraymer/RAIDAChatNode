using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using RAIDAChatNode.Utils;

namespace RAIDAChatNode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //new TimerCleanUp().Start();
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(File.ReadLines("wwwroot\\url.txt").First())
                .Build();
    }
}
