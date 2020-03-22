using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using PuckFeedFunctionApp.BO;
using System.Threading.Tasks;

namespace PuckFeedFunctionApp
{
    public static class ProductBlogTriggerFunction
    {
        [FunctionName("OnProductFileUploaded")]
        public static async Task Run([BlobTrigger("product-container/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            string cs = Environment.GetEnvironmentVariable("MyConnectionString", EnvironmentVariableTarget.Process);
            var data = String.Empty;
            var dbAccess = new DataAccess(cs);

            if (myBlob.Length > 0)
            {
                using var reader = new StreamReader(myBlob);
                var lineNumber = 1;
                var line = await reader.ReadLineAsync();
                StringBuilder sb = new StringBuilder();
                while (line != null)
                {
                    line = await reader.ReadLineAsync();
                    sb.AppendLine(line);
                    lineNumber++;
                }

                data = sb.ToString();
                var raw = data.Split('\n');
                int rowSkip = 1;
                foreach (var item in raw)
                {
                    if (rowSkip > 2)
                    {
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        String[] categoryData = CSVParser.Split(item);


                        if (categoryData.Length != 13)
                        {
                            continue;
                        }

                        Product Obj = new Product()
                        {
                            ProductId = Common.GetInt(categoryData.GetValue(0).ToString()),
                            Model = Common.GetString(categoryData.GetValue(1).ToString()),
                            EAN = Common.GetString(categoryData.GetValue(2).ToString()),
                            Name = Common.GetString(categoryData.GetValue(3).ToString()),
                            Description = Common.GetString(categoryData.GetValue(4).ToString()),
                            Dimension = Common.GetString(categoryData.GetValue(5).ToString()),
                            Price = Common.GetString(categoryData.GetValue(6).ToString()),
                            DeliveryCode = Common.GetString(categoryData.GetValue(7).ToString()),
                            Quantity = Common.GetString(categoryData.GetValue(8).ToString()),
                            Categories = Common.GetString(categoryData.GetValue(9).ToString()),
                            Options = Common.GetString(categoryData.GetValue(10).ToString()),
                            MOQ = Common.GetString(categoryData.GetValue(11).ToString()),
                            ImagesUrl = Common.GetString(categoryData.GetValue(12).ToString())
                        };

                        try
                        {

                            await dbAccess.UpsertProduct(Obj.ProductId, Obj.Model, Obj.EAN, Obj.Name, Obj.Description, Obj.Dimension, Obj.Price, Obj.DeliveryCode, Obj.Quantity, Obj.Categories, Obj.Options, Obj.MOQ, Obj.ImagesUrl);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"EXCEPTION @ OnProductFileUploaded: {ex.Message}");
                        }
                    }

                    rowSkip++;
                }
            }

            log.Info($"OnProductFileUploaded has been trigger with blob Name:{name} \n Size: {myBlob.Length} Bytes");

            //var container = blobClient.GetContainerReference(containerName);
            //var blockBlob = container.GetBlockBlobReference(name);
            //return blockBlob.DeleteIfExists();

        }
    }

}
