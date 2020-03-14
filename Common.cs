using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace puckatorFeedLoader
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
    }
}
