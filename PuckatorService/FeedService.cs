using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedCreator
{
    public class FeedService
    {

        public string DownLoadStringData(string url)
        {
            System.Net.WebClient webClient = new System.Net.WebClient();
            return webClient.DownloadString(url);
        }

        public void DownLoadFile(string url,string fileName)
        {
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.DownloadFile(url,fileName);
        }

        public void CreateCSV(DataTable dt, string aDestPath, string aTitle)
        {

            if (!Directory.Exists(aDestPath))
                throw new DirectoryNotFoundException($"Directory not found: {aDestPath}");


            string filePath = Path.Combine(aDestPath, aTitle);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();
            List<string> CsvRow = new List<string>();



            foreach (DataColumn c in dt.Columns)
            {

                CsvRow.Add(c.ColumnName);
            }
            sb.AppendLine(string.Join(delimiter, CsvRow));



            foreach (DataRow r in dt.Rows)
            {
                CsvRow.Clear();


                foreach (DataColumn c in dt.Columns)
                {

                    CsvRow.Add(r[c].ToString());
                }

                sb.AppendLine(string.Join(delimiter, CsvRow));
            }

            File.AppendAllText(filePath, sb.ToString());


        }
    }
}