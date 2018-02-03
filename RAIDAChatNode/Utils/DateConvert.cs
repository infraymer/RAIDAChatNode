using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
