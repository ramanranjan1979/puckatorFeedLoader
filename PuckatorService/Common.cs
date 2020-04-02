using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedCreator
{
    public static class Common
    {
        public static string GetString(string rawString)
        {
            return rawString.ToString().Replace('\"', ' ').Trim();
        }

        public static int GetInt(string rawString)
        {
            return int.Parse(rawString.ToString().Replace('\"', ' ').Trim());
        }

        public static string GetFileNameWithTimestamp(string extension)
        {
            return $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.{extension}";
        }

        public static string GetBaseDirectory()
        {
            return AppContext.BaseDirectory.Replace(@"\bin\Debug\", String.Empty);
        }

        public static String GetCurrentTimestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        
    }
}
