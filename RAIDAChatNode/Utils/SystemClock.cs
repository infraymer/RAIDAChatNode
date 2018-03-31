using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using RAIDAChatNode.DTO.Configuration;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;

namespace RAIDAChatNode.Utils
{
    public class SystemClock
    {
        /// <summary>
        /// Count of seconds with 01.01.1970
        /// </summary>
        public static long CurrentTime { get; private set; }

        private int offset = 0,
            timeCleanUp = 0;
        private int ResyncTime = MainConfig.SyncWorldTime, //Sync time every 24 hours
                    CleanUp = MainConfig.CleanUpTimer; //Default 1 hours = 3600 seconds
        
        public SystemClock()
        {
            CurrentTime = SyncCurrentTime().Result;
            new Timer(TimerTickCleanup, null, 0, 1000);
        }
        
        private void TimerTickCleanup(object state)
        {
            CurrentTime++;
            offset++;
            if (offset >= ResyncTime)
            {
                offset = 0;
                CurrentTime = SyncCurrentTime().Result;
            }

            timeCleanUp++;
            if (timeCleanUp >= CleanUp)
            {
                timeCleanUp = 0;
                TimerTickCleanup();
            }
        }
        
        
        private async Task<long> SyncCurrentTime()
        {
            List<long> timeStandart = new List<long>();
            const string patternStamp = "\\d{10}";
            int timeOut = 5; //Seconds
            
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
                    string response = await t.WithTimeout(timeOut).GetAsync().ReceiveString();
                    response = Regex.Match(response, patternStamp).Value;
                    if (long.TryParse(response, out long time))
                    {
                        time = DateConvert.validateTimestamp(time);
                        timeStandart.Add(time);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
            }

            if (timeStandart.Count == 0)
            {
                LoadConfiguration.CloseApp("Time is not syncronize");
            }
            
            long averageTime = (long) Math.Truncate(timeStandart.Average());
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Synchronized time servers {timeStandart.Count}/{timeServers.Length}: {DateTimeOffset.FromUnixTimeSeconds(averageTime).LocalDateTime.ToString("G")} ");
            Console.ResetColor();

            
            
            return timeStandart.Count != timeServers.Length ? averageTime + timeOut : averageTime;
        }
        
        private void TimerTickCleanup()
        {
            long dateNow = CurrentTime;
            using (var db = new RaidaContext())
            {

                //Cleanup 'death_date' message
                db.Shares.Where(it => it.death_date <= dateNow)
                    .ToList()
                    .ForEach(delegate (Shares s)
                        {
                            db.Shares.Remove(s);
                        }
                    );

                //Cleanup blocked transactions
                db.Transactions.Where(it => it.rollbackTime <= dateNow).ToList()
                    .ForEach(delegate (Transactions t)
                        {
                            db.Transactions.Remove(t);
                        }
                    );

                db.SaveChanges();
            }
        }
    }
}