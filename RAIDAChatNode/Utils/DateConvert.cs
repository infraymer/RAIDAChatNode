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
    }
}
