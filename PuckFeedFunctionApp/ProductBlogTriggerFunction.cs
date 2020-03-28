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

                string data = sb.ToString();
                var raw = data.Split('\n');
                //int rowSkip = 1;
                foreach (var item in raw)
                {
                    //if (rowSkip > 2)
                    //{
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        String[] rawData = CSVParser.Split(item);


                        if (rawData.Length != 13)
                        {
                            continue;
                        }

                        Product Obj = new Product()
                        {
                            ProductId = Common.GetInt(rawData.GetValue(0).ToString()),
                            Model = Common.GetString(rawData.GetValue(1).ToString()),
                            EAN = Common.GetString(rawData.GetValue(2).ToString()),
                            Name = Common.GetString(rawData.GetValue(3).ToString()),
                            Description = Common.GetString(rawData.GetValue(4).ToString()),
                            Dimension = Common.GetString(rawData.GetValue(5).ToString()),
                            Price = Common.GetString(rawData.GetValue(6).ToString()),
                            DeliveryCode = Common.GetString(rawData.GetValue(7).ToString()),
                            Quantity = Common.GetString(rawData.GetValue(8).ToString()),
                            Categories = Common.GetString(rawData.GetValue(9).ToString()),
                            Options = Common.GetString(rawData.GetValue(10).ToString()),
                            MOQ = Common.GetString(rawData.GetValue(11).ToString()),
                            ImagesUrl = Common.GetString(rawData.GetValue(12).ToString())
                        };

                        try
                        {

                            await dbAccess.UpsertProduct(Obj.ProductId, Obj.Model, Obj.EAN, Obj.Name, Obj.Description, Obj.Dimension, Obj.Price, Obj.DeliveryCode, Obj.Quantity, Obj.Categories, Obj.Options, Obj.MOQ, Obj.ImagesUrl);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"EXCEPTION @ OnProductFileUploaded: {ex.Message}");
                        }
                    //}

                    //rowSkip++;
                }
            }

            log.Info($"OnProductFileUploaded has been trigger with blob Name:{name} \n Size: {myBlob.Length} Bytes");

        }

    }

    public static class ProductCategoryBlogTriggerFunction
    {
        [FunctionName("OnProductCategoryFileUploaded")]
        public static async Task Run([BlobTrigger("category-container/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
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
                foreach (var item in raw)
                {

                    Regex expression = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    String[] rawData = expression.Split(item);

                    Category Obj = new Category()
                    {
                        CategoryId = Common.GetInt(rawData.GetValue(0).ToString()),
                        ParentCategoryId = Common.GetInt(rawData.GetValue(1).ToString()),
                        Description = Common.GetString(rawData.GetValue(2).ToString()),
                        Active = true
                    };

                    try
                    {

                        await dbAccess.UpsertCategory(Obj.CategoryId, Obj.ParentCategoryId, Obj.Description, Obj.Active);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"EXCEPTION @ OnProductCategoryFileUploaded: {ex.Message}");
                    }



                }
            }
            log.Info($"OnProductCategoryFileUploaded has been trigger with blob Name:{name} \n Size: {myBlob.Length} Bytes");

        }

    }

}
