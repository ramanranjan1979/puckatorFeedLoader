using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace puckatorFeedLoader
{
    class Service
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
    }
}