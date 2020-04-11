using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PuckatorService
{
    public class AzureService : IDisposable
    {
        private string _storageAccount = string.Empty;

        public AzureService()
        {
            _storageAccount = System.Configuration.ConfigurationManager.AppSettings["AzureStorageAccount"];
        }

       

        public AzureService(string Connection)
        {
            _storageAccount = Connection;
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

        public async Task<string> AddBlob(Stream fs, string destinationContainer, string fileKeyName)
        {
            CloudStorageAccount sa = CloudStorageAccount.Parse(_storageAccount);
            CloudBlobClient bc = sa.CreateCloudBlobClient();
            CloudBlobContainer conainer = bc.GetContainerReference(destinationContainer);
            //conainer.CreateIfNotExists();
            CloudBlockBlob blob = conainer.GetBlockBlobReference(fileKeyName);
            await blob.UploadFromStreamAsync(fs); 
            return blob.Uri.AbsoluteUri;
        }

        public async Task DeleteBlob(string destinationContainer, string fileKeyName)
        {
            CloudStorageAccount sa = CloudStorageAccount.Parse(_storageAccount);
            CloudBlobClient bc = sa.CreateCloudBlobClient();
            CloudBlobContainer conainer = bc.GetContainerReference(destinationContainer);
            CloudBlockBlob blob = conainer.GetBlockBlobReference(fileKeyName);
            blob.DeleteAsync();
        }

        public async Task AddMessageInQueue(string destinationQueueName, string messageId)
        {
            CloudStorageAccount sa = CloudStorageAccount.Parse(_storageAccount);
            CloudQueueClient client = sa.CreateCloudQueueClient();
            CloudQueue myqueue = client.GetQueueReference(destinationQueueName);
            //myqueue.CreateIfNotExists();
            myqueue.AddMessageAsync(new CloudQueueMessage(messageId));

        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}