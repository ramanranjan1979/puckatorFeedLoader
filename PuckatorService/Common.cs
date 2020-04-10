using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PuckatorService
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

        public static string GetFileNameWithTimestamp(string fileName, string extension)
        {
            return $"{fileName}-{GetCurrentTimestamp()}.{extension}";
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

        public static  async Task<Stream> GetImageAsStream(string urlImage, string urlBase)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(urlBase);
            var response = await client.GetAsync(urlImage);
            return await response.Content.ReadAsStreamAsync();
        }
    }

    public enum LogType
    {
        PRODUCT_CODE_FILE_PULL_FROM_SOURCE = 1,
        PRODUCT_IMAGE_FILE_PULL_FROM_SOURCE = 2,
        PRODUCT_CATEGORY_FILE_PULL_FROM_SOURCE = 3,
        PRODUCT_FILE_PULL_FROM_SOURCE = 4,
    }
}
