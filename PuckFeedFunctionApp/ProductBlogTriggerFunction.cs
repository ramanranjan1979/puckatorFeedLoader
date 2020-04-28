using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using FeedFunctionApp.BO;
using System.Threading.Tasks;
using PuckatorService;
using System.Collections.Generic;

namespace FeedFunctionApp
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
                log.Info($"OnProductFileUploaded has been trigger with blob Name:{name} \n Size: {myBlob.Length} Bytes");

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

                List<Product> productList = new List<Product>();

                List<string> errorProductList = new List<string>();

                foreach (var item in raw)
                {

                    //Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    string[] rawData = CSVParser.Split(item);


                    if (rawData.Length != 13)
                    {
                        errorProductList.Add(rawData.GetValue(0).ToString());
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

                    productList.Add(Obj);

                   
                }


                foreach (var Obj in productList)
                {
                    try
                    {

                        await dbAccess.UpsertProduct(Obj.ProductId, Obj.Model, Obj.EAN, Obj.Name, Obj.Description, Obj.Dimension, Obj.Price, Obj.DeliveryCode, Obj.Quantity, Obj.Categories, Obj.Options, Obj.MOQ, Obj.ImagesUrl);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"EXCEPTION @ OnProductFileUploaded: {ex.Message}");
                    }
                }
            }

            if (myBlob.Length > 0)
            {
                await new AzureService(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process)).DeleteBlob("product-container", name);
                log.Info($"Blob Name:{name} \n Size: {myBlob.Length} Bytes has been delete from product-container ");
            }


        }

    }
    public static class ProductCategoryBlogTriggerFunction
    {
        [FunctionName("OnProductCategoryFileUploaded")]
        public static async Task Run([BlobTrigger("category-container/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            string cs = Environment.GetEnvironmentVariable("MyConnectionString", EnvironmentVariableTarget.Process);
            var dbAccess = new DataAccess(cs);

            if (myBlob.Length > 0)
            {
                log.Info($"OnProductCategoryFileUploaded has been trigger with blob Name:{name} \n Size: {myBlob.Length} Bytes");

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
                foreach (var item in raw)
                {
                    try
                    {

                        Regex expression = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        string[] rawData = expression.Split(item);

                        if (rawData.Length != 4)
                            continue;

                        Category Obj = new Category()
                        {
                            CategoryId = Common.GetInt(rawData.GetValue(0).ToString()),
                            ParentCategoryId = Common.GetInt(rawData.GetValue(1).ToString()),
                            Description = Common.GetString(rawData.GetValue(2).ToString()),
                            Active = true
                        };

                        await dbAccess.UpsertCategory(Obj.CategoryId, Obj.ParentCategoryId, Obj.Description, Obj.Active);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"EXCEPTION @ OnProductCategoryFileUploaded: {ex.Message}");
                    }
                }
            }

            if (myBlob.Length > 0)
            {
                await new AzureService(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process)).DeleteBlob("category-container", name);
                log.Info($"Blob Name:{name} \n Size: {myBlob.Length} Bytes has been delete from category-container ");
            }
        }

    }
    public static class ProductImageBlogTriggerFunction
    {
        [FunctionName("OnProductImageFileUploaded")]
        public static async Task Run([BlobTrigger("image-container/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            string cs = Environment.GetEnvironmentVariable("MyConnectionString", EnvironmentVariableTarget.Process);
            var dbAccess = new DataAccess(cs);

            if (myBlob.Length > 0)
            {
                log.Info($"OnProductImageFileUploaded has been trigger with blob Name:{name} \n Size: {myBlob.Length} Bytes");

                using var reader = new StreamReader(myBlob);
                var line = await reader.ReadLineAsync();
                StringBuilder sb = new StringBuilder();
                while (line != null)
                {
                    line = await reader.ReadLineAsync();
                    sb.AppendLine(line);
                }

                var raw = sb.ToString().Split('\n');
                int rowSkip = 1;
                foreach (var item in raw)
                {
                    if (rowSkip > 1)
                    {
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        string[] fields = CSVParser.Split(item);

                        ProductImage pi = new ProductImage
                        {
                            ImageList = new List<Image>()
                        };

                        int totalImageCount = fields.Length - 1;
                        pi.ProductModel = Common.GetString(fields.GetValue(0).ToString());

                        for (int i = 1; i <= totalImageCount; i++)
                        {
                            pi.ImageList.Add(new Image()
                            {
                                FileName = Common.GetString(fields.GetValue(i).ToString()),
                                IsMain = i == 1,
                                Number = i
                            });
                        }

                        try
                        {
                            foreach (var image in pi.ImageList)
                            {
                                if (image.FileName != string.Empty)
                                {
                                    await dbAccess.UpsertProductImage(pi.ProductModel, image.FileName, image.Number, image.IsMain, true);
                                }
                            }

                            //Adding ProductModel In Queue for Image Download Trigger

                            await new AzureService(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process)).AddMessageInQueue("imagedownloadqueue", pi.ProductModel);
                            log.Info($"A Message {pi.ProductModel} has been added in Queue Name: imagedownloadqueue");

                        }
                        catch (Exception ex)
                        {
                            log.Error($"EXCEPTION @ OnProductImageFileUploaded: {ex.Message}");
                        }
                    }

                    rowSkip++;
                }
            }



            if (myBlob.Length > 0)
            {
                await new AzureService(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process)).DeleteBlob("image-container", name);
                log.Info($"Blob Name:{name} \n Size: {myBlob.Length} Bytes has been delete from image-container ");
            }
        }

    }
    public static class ProductCodeBlogTriggerFunction
    {
        [FunctionName("OnProductCodeFileUploaded")]
        public static async Task Run([BlobTrigger("barcode-container/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            string cs = Environment.GetEnvironmentVariable("MyConnectionString", EnvironmentVariableTarget.Process);
            var dbAccess = new DataAccess(cs);

            if (myBlob.Length > 0)
            {
                log.Info($"OnProductCodeFileUploaded has been trigger with blob Name:{name} \n Size: {myBlob.Length} Bytes");

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
                var raw = sb.ToString().Split('\n');
                int rowSkip = 1;
                foreach (var item in raw)
                {
                    if (rowSkip > 1)
                    {
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        string[] fields = CSVParser.Split(item);

                        Productcode Obj = new Productcode()
                        {
                            ProductModel = Common.GetString(fields.GetValue(0).ToString()),
                            code = Common.GetString(fields.GetValue(1).ToString()),

                        };

                        try
                        {
                            await dbAccess.UpsertProductCode(Obj.ProductModel, Obj.code, true);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"EXCEPTION @ OnProductImageFileUploaded: {ex.Message}");
                        }

                    }

                    rowSkip++;
                }
            }



            if (myBlob.Length > 0)
            {
                await new AzureService(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process)).DeleteBlob("barcode-container", name);
                log.Info($"Blob Name:{name} \n Size: {myBlob.Length} Bytes has been delete from barcode-container ");
            }
        }

    }

    public static class ImageQueueMessageTriggerFunction
    {
        [FunctionName("OnProductCodeInMessageQueue")]
        public static async Task Run([QueueTrigger("imagedownloadqueue", Connection = "AzureWebJobsStorage")]string queueItem, TraceWriter log)
        {
            string cs = Environment.GetEnvironmentVariable("MyConnectionString", EnvironmentVariableTarget.Process);
            var dbAccess = new DataAccess(cs);

            if (queueItem.Length > 0)
            {
                log.Info($"OnProductCodeInMessageQueue has been trigger with queue name imagedownloadqueue and message id:{queueItem}");

                var containerName = $"images/{queueItem}";

                var dataSet = dbAccess.GetProductImageFileNames(queueItem);

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        try
                        {
                            var blobName = dbAccess.GetRowValue(row, "FileName");
                            var imageNumber = int.Parse(dbAccess.GetRowValue(row, "Number"));
                            var isMain = bool.Parse(dbAccess.GetRowValue(row, "IsMain"));
                            var isActive = bool.Parse(dbAccess.GetRowValue(row, "Active"));

                            Stream fs = await Common.GetImageAsStream(blobName, @"http://www.puckator-dropship.co.uk/gifts/images/");

                            if (Common.ConvertBytesKilobytes(fs.Length) > 0)
                            {
                                using var az = new AzureService(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));
                                var url = await az.AddBlob(fs, containerName, blobName);
                                await dbAccess.UpsertProductImage(queueItem, blobName, imageNumber, isMain, isActive, DateTime.Now, url);
                            }
                        }

                        catch (Exception ex)
                        {
                            log.Error($"Exception occurred in OnProductCodeInMessageQueue :{ex.Message}");
                        }
                    }
                }
            }

            //NO Need : Looks like message is getting deleted after been served

            //if (queueItem.Length > 0)
            //{
            //    await new AzureService(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process)).DeleteMessageFromQueue("imagedownloadqueue", queueItem);
            //    log.Info($"A Message {queueItem} has been deleted from queue name: imagedownloadqueue");
            //}
        }



    }


}
