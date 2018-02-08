using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RAIDAChatNode.Utils
{
    public class TimerCleanUp
    {
        private long timeLoop { get; set; }
        private Timer timer;

        public TimerCleanUp()
        {
            timeLoop = 3600000; //Default 1 hours = 3600000 milliseconds
        }

        /// <summary>
        /// Timer for clean 'death_date' message and blocked transactions
        /// </summary>
        /// <param name="timeLoop">await time (milliseconds)</param>
        public TimerCleanUp(long timeLoop)
        {
            this.timeLoop = timeLoop;
        }

        public void Start()
        {
            
            timer = new Timer(TimerTickCleanup, null, 0, timeLoop);
        }

        public void Stop()
        {
            timer.Dispose();
        }

        private void TimerTickCleanup(object state)
        {
            long dateNow = DateTimeOffset.Now.ToUnixTimeSeconds();
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
