using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
using Microsoft.EntityFrameworkCore;
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
            
            try
            {
                SerializeMainConfig serConf = JsonConvert.DeserializeObject<SerializeMainConfig>(config);
                DeserializeObject.IsValid(serConf);

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
            if ((!MainConfig.DB.NameDB.Equals(SupportedDB.SQLite, StringComparison.OrdinalIgnoreCase)) 
                && (!(MainConfig.DB.ConnectionString.ToLower().Contains("password") && MainConfig.DB.ConnectionString.ToLower().Contains("user"))) )
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
                        tmp += $"user = {user}; password = {pass};";
                        break;
                }

                MainConfig.DB.ConnectionString = tmp;   
            }

            int i = 1;
            while (i <= MainConfig.DB.CntConnect)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Connection to DataBase... {i}/{MainConfig.DB.CntConnect}");
                try
                {
                    using (new RaidaContext()){
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("DataBase connection successful");
                        break;
                    }
                    
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Database connection is fail ({i}/{MainConfig.DB.CntConnect}).\r\nError: {e.Message}");
                }
                i++;
                Thread.Sleep(1000);
            }

            if (i > MainConfig.DB.CntConnect)
            {
                CloseApp($"Database connection is fail");   
            }
        }
        
        private static string ReadLineHidden() {
            var sb = new StringBuilder();
            Console.ReadKey(true); //Without it always reading Enter
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
            Console.WriteLine($"{errorMessage}!\r\nServer is was stopped!\r\nPress Enter...");
            Console.ReadLine();
            Environment.Exit(-1);
        }
    }
}