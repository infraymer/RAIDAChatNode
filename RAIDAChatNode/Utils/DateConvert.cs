using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Internal;


namespace RAIDAChatNode.Utils
{
    public static class DateConvert
    {
        public static long validateTimestamp(long timestamp)
        {
            timestamp = Math.Abs(timestamp);
            if (timestamp > 9999999999)
            {
                timestamp = long.Parse(timestamp.ToString().Substring(0, 10));
            }
            return timestamp;
        }

        public static async Task<bool> EqualsCurrentTime()
        {
            long timeStampNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long timeStampDelta = 60; //+- 1 min for start app
            List<long> timeStandart = new List<long>();
            const string patternStamp = "\\d{10}";
            
            string[] timeServers = new[] {
                "https://time100.ru/api.php",
                "http://www.direct-time.ru/track.php?id=time_utc",
                "https://www.time.gov/actualtime.cgi",
                "https://www.timeanddate.com/scripts/ts.php?",
                "https://time.is/t/?ru.0.117.135.0p.300.44a.0.0."
            };

            foreach (string t in timeServers)
            {
                try
                {
                    string response = await t.WithTimeout(5).GetAsync().ReceiveString();
                    response = Regex.Match(response, patternStamp).Value;
                    if (long.TryParse(response, out long time))
                    {
                        time = validateTimestamp(time);
                        timeStandart.Add(time);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
            }

            double averageTime = Math.Truncate(timeStandart.Average());

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Synchronized time servers {timeStandart.Count}/{timeServers.Length} ");
            Console.ResetColor();
            
            return timeStampNow > averageTime - timeStampDelta && timeStampNow < averageTime + timeStampDelta;
        }
        
        
    }
}
