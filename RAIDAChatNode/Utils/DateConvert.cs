using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAIDAChatNode.Utils
{
    public static class DateConvert
    {
        //public static long convertToUnixTimestamp(DateTime now)
        //{
        //    now = now.ToUniversalTime();
        //    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //    TimeSpan diff = now - origin;
        //    return (long)Math.Floor(diff.TotalSeconds);
        //}

        //public static DateTime convertFromUnixTimestamp(long timestamp)
        //{
        //    timestamp = validateTimestamp(timestamp);
        //    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //    return origin.AddSeconds(timestamp);
        //}

        /// <summary>
        /// Validate input timestamp range(0..9999999999). 
        /// </summary>
        /// <param name="timestamp">input timestamp</param>
        /// <returns></returns>
        public static long validateTimestamp(long timestamp)
        {
            timestamp = Math.Abs(timestamp);
            if (timestamp > 9999999999)
            {
                timestamp = long.Parse(timestamp.ToString().Substring(0, 10));
            }
            return timestamp;
        }
    }
}
