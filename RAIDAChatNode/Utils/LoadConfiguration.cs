using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
using Newtonsoft.Json;
using RAIDAChatNode.DTO.Configuration;
using RAIDAChatNode.Model;

namespace RAIDAChatNode.Utils
{
    public class LoadConfiguration
    {
        public static void Load()
        {
            string config = "";
            try
            {
                config = File.ReadAllText("appsettings.json");
            }
            catch (Exception ex)
            {
                CloseApp("File of configuration is not found or error opening");
            }

            Console.WriteLine(config);
            
            
            try
            {
                SerializeMainConfig serConf = JsonConvert.DeserializeObject<SerializeMainConfig>(config);

                if (serConf.Connection.IP == null ||  !Regex.IsMatch(serConf.Connection.IP, "\\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\\.|$)){4}\\b") || serConf.TrustedServers.Count == 0)
                {
                    throw new Exception("IP address is not valid OR count trusted servers = 0");
                }

                MainConfig.Mapping(serConf);
                CheckDB();
            }
            catch (Exception ex)
            {
                CloseApp(ex.Message);
            }            
        }

        private static void CheckDB()
        {
            if (!MainConfig.DB.NameDB.Equals(SupportedDB.SQLite, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Database authenticatoin:");
                Console.Write("User name: ");
                string user = Console.ReadLine();
                Console.Write("Password: ");
                string pass = ReadLineHidden();

                string tmp = MainConfig.DB.ConnectionString.Trim();
                if (!tmp[tmp.Length - 1].Equals(';'))
                {
                    tmp += ";";
                }
                
                switch (MainConfig.DB.NameDB)
                {
                    case SupportedDB.MSSQL:
                    case SupportedDB.Oracle:
                        tmp += $"user id = {user}; password = {pass};";
                        break;
                    case SupportedDB.MySQL:
                        tmp += $"Uid = {user}; pwd = {pass};";
                        break;
                }

                MainConfig.DB.ConnectionString = tmp;

                try
                {
                    using (new RaidaContext()){}
                }
                catch (Exception e)
                {
                    CloseApp($"Database connection is fail. Error: {e.Message}");
                }
            }
        }
        
        private static string ReadLineHidden() {
            var sb = new StringBuilder();
            while (true) {
                var pwd = Console.ReadKey(true);
                if (pwd.Key == ConsoleKey.Enter) {
                    Console.WriteLine();
                    break;
                }
                else if (pwd.Key == ConsoleKey.Backspace) {
                    if (sb.Length > 0) {
                        sb.Length--;
                    }
                }

                sb.Append(pwd.KeyChar);
            }

            return sb.ToString();
        }
        
        private class SupportedDB
        {
            public const string SQLite = "SQLite";
            public const string MSSQL = "MSSQL";
            
            public const string MySQL = "MySQL";
            public const string Oracle = "Oracle";
        }
        
        /// <summary>
        /// Close application with message of error
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        public static void CloseApp(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{errorMessage}! Server is was stopped! Press enter...");
            Console.ReadLine();
            Environment.Exit(-1);
        }
    }
}