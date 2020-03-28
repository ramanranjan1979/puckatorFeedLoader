using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace puckatorFeedLoader
{
    public class AzureService : IDisposable
    {
        private string _storageAccount = string.Empty;

        public AzureService()
        {
            _storageAccount = System.Configuration.ConfigurationManager.AppSettings["AzureStorageAccount"];
        }

        public void AddBlob(string filePath, string destinationContainer, string fileKeyName)
        {
            CloudStorageAccount sa = CloudStorageAccount.Parse(_storageAccount);
            CloudBlobClient bc = sa.CreateCloudBlobClient();
            CloudBlobContainer conainer = bc.GetContainerReference(destinationContainer);

            conainer.CreateIfNotExists();

            CloudBlockBlob blob = conainer.GetBlockBlobReference(fileKeyName);

            using (var fs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                blob.UploadFromStream(fs);
            }
        }

        public void DeleteBlob(string destinationContainer, string fileKeyName)
        {
            CloudStorageAccount sa = CloudStorageAccount.Parse(_storageAccount);
            CloudBlobClient bc = sa.CreateCloudBlobClient();
            CloudBlobContainer conainer = bc.GetContainerReference(destinationContainer);
            CloudBlockBlob blob = conainer.GetBlockBlobReference(fileKeyName);
            blob.Delete();            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}