using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using RAIDAChatNode.Utils;

namespace RAIDAChatNode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (DateConvert.EqualsCurrentTime().Result)
            {
                //new TimerCleanUp().Start();
                BuildWebHost(args).Run();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("RChat server is not started.\r\n" +
                              "The date or time of the PC is incorrect.\r\n" +
                              "Please check and set the correct date and time.\r\n" +
                              "Press any key to continue and quit app.");
                Console.ReadKey();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(File.ReadLines("wwwroot\\url.txt").First())
                .Build();
    }
}
