using PuckatorService;
using PuckatorService.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PuckatorFeedCreator
{
    public class PuckatorFeedCreationService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private static string UserName = string.Empty;
        private static string password = string.Empty;

        private static string categoryUrl = string.Empty;
        private static string productUrl = string.Empty;
        private static string productBarcodeUrl = string.Empty;
        private static string productImageUrl = string.Empty;

        private static List<Catalogue> myCatalogue = null;

        private static FeedService feedService = null;
        private static EmailFeedService emailService = null;
        private static int catalogueLevel = 0;

        private static DataAccess dbAccess = null;

        private static string productFilePath = string.Empty;
        private static string productImagePath = string.Empty;
        private static string productBarcodeFilePath = string.Empty;

        private static string Product_SourceFolder = string.Empty;
        private static string ProductCategory_SourceFolder = string.Empty;
        private static string ProductImage_SourceFolder = string.Empty;
        private static string ProductCode_SourceFolder = string.Empty;

        private static string ProductDestinationContainer = string.Empty;
        private static string CategoryDestinationContainer = string.Empty;
        private static string ImageDestinationContainer = string.Empty;
        private static string BarcodeDestinationContainer = string.Empty;

        private static Timer productFileCreationTimer;
        private static Timer productCategoryFileCreationTimer;
        private static Timer productImageFileCreationTimer;
        private static Timer productBarcodeFileCreationTimer;
        private static Timer notificationTimer;

        private static SettingList _settingList = null;
        private static List<KeyValuePair<string, string>> messageList;

        private const int pollInterval = 120000; //120 seconds
        //private const int pollInterval = 600000; // 10 minutes
        //private const int pollInterval = 10800000; // 3 HRS



        public void Start()
        {
            log.Info("PuckatorFeedCreationService service has started");

            Setup();

            LoadSetting();

            LoadTimer();
        }

        private void LoadSetting()
        {
            //var data = dbAccess.GetSetting(); 
        }

        private static void Setup()
        {
            feedService = new FeedService();
            emailService = new EmailFeedService(bool.Parse(System.Configuration.ConfigurationManager.AppSettings["TESTMODE"]));
            dbAccess = new DataAccess(System.Configuration.ConfigurationManager.AppSettings["DbConnection"]);

            UserName = System.Configuration.ConfigurationManager.AppSettings["FeedLoadUserName"];
            password = System.Configuration.ConfigurationManager.AppSettings["FeedLoadUserPassword"];
            categoryUrl = System.Configuration.ConfigurationManager.AppSettings["CategoryDataUrl"];
            productUrl = System.Configuration.ConfigurationManager.AppSettings["ProductDataUrl"];
            productBarcodeUrl = System.Configuration.ConfigurationManager.AppSettings["EANDataUrl"];
            productImageUrl = System.Configuration.ConfigurationManager.AppSettings["ImageDataUrl"];

            productFilePath = Path.Combine(Common.GetBaseDirectory(), System.Configuration.ConfigurationManager.AppSettings["ProductFilePath"]);
            productImagePath = System.Configuration.ConfigurationManager.AppSettings["ProductImagesFilePath"];
            productBarcodeFilePath = System.Configuration.ConfigurationManager.AppSettings["ProductBarcodeFilePath"];

            Product_SourceFolder = System.Configuration.ConfigurationManager.AppSettings["Product_SourceFolder"];
            ProductDestinationContainer = System.Configuration.ConfigurationManager.AppSettings["ProductDestinationContainer"];

            ProductCategory_SourceFolder = System.Configuration.ConfigurationManager.AppSettings["ProductCategory_SourceFolder"];
            CategoryDestinationContainer = System.Configuration.ConfigurationManager.AppSettings["CategoryDestinationContainer"];

            ProductImage_SourceFolder = System.Configuration.ConfigurationManager.AppSettings["ProductImage_SourceFolder"];
            ImageDestinationContainer = System.Configuration.ConfigurationManager.AppSettings["ImageDestinationContainer"];

            ProductCode_SourceFolder = System.Configuration.ConfigurationManager.AppSettings["ProductCode_SourceFolder"];
            BarcodeDestinationContainer = System.Configuration.ConfigurationManager.AppSettings["BarcodeDestinationContainer"];

            myCatalogue = new List<Catalogue>();
            messageList = new List<KeyValuePair<string, string>>();

        }

        public void Stop()
        {
            log.Info("PuckatorFeedCreationService service has stopoped");
        }

        private static void LoadTimer()
        {
            productFileCreationTimer = new Timer(new TimerCallback(CreateProductFile), null, 0, pollInterval);
            productCategoryFileCreationTimer = new Timer(new TimerCallback(CreateCategoryFileFile), null, 0, pollInterval);
            productImageFileCreationTimer = new Timer(new TimerCallback(CreateImageFile), null, 0, pollInterval);
            productBarcodeFileCreationTimer = new Timer(new TimerCallback(CreateBarcodeFile), null, 0, pollInterval);
            notificationTimer = new Timer(new TimerCallback(SendNotification), null, 0, pollInterval);
        }

        private static void CreateProductFile(object state)
        {
            try
            {
                productFileCreationTimer.Change(Timeout.Infinite, Timeout.Infinite);
                log.Info($"CreateProductFile load has been ticked @ {DateTime.Now.ToLongTimeString()}");

                string requestUrl = $"{productUrl}?email={UserName}&passwd={password}&action=full";
                // var data = "<br>Dropship Feed Error: You must wait 2 hours between product feed requests.";
                var data = feedService.DownLoadStringData(requestUrl);
                if (data.Length > 0)
                {
                    if (data.ToUpper() == "<br>Dropship Feed Error: You must wait 2 hours between product feed requests.".ToUpper())
                    {

                        messageList.Add(new KeyValuePair<string, string>($"CreateProductFile#{Guid.NewGuid()}", $"Product File Result: {data}"));
                        return;


                        //using (var reader = new StreamReader(@"D:\GIT\puckatorFeedLoader\File\Product\Product.csv"))
                        //{
                        //    StringBuilder sb = new StringBuilder();
                        //    while (!reader.EndOfStream)
                        //    {
                        //        var line = reader.ReadLine();
                        //        sb.AppendLine(line);
                        //    }

                        //    data = sb.ToString();
                        //}
                    }

                    DataTable dt = new DataTable();

                    dt.Columns.Add("products_id");
                    dt.Columns.Add("model");
                    dt.Columns.Add("ean");
                    dt.Columns.Add("name");
                    dt.Columns.Add("description");
                    dt.Columns.Add("dimension");
                    dt.Columns.Add("price");
                    dt.Columns.Add("delivery_code");
                    dt.Columns.Add("quantity");
                    dt.Columns.Add("categories");
                    dt.Columns.Add("options");
                    dt.Columns.Add("minimum_order_quantity");
                    dt.Columns.Add("image_url");

                    DataRow dr;


                    var raw = data.Split('\n');
                    int rowSkip = 1;
                    foreach (var item in raw)
                    {
                        dr = dt.NewRow();

                        if (rowSkip > 2)
                        {

                            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                            String[] productData = CSVParser.Split(item);
                            if (productData.Length != 13)
                            {
                                continue;
                            }

                            dr[0] = Common.GetInt(productData.GetValue(0).ToString());
                            dr[1] = Common.GetString(productData.GetValue(1).ToString());
                            dr[2] = Common.GetString(productData.GetValue(2).ToString());
                            dr[3] = Common.GetString(productData.GetValue(3).ToString());
                            dr[4] = Common.GetString(productData.GetValue(4).ToString());
                            dr[5] = Common.GetString(productData.GetValue(5).ToString());
                            dr[6] = Common.GetString(productData.GetValue(6).ToString());
                            dr[7] = Common.GetString(productData.GetValue(7).ToString());
                            dr[8] = Common.GetString(productData.GetValue(8).ToString());
                            dr[9] = Common.GetString(productData.GetValue(9).ToString());
                            dr[10] = Common.GetString(productData.GetValue(10).ToString());
                            dr[11] = Common.GetString(productData.GetValue(11).ToString());
                            dr[12] = Common.GetString(productData.GetValue(12).ToString());


                            dt.Rows.Add(dr);
                        }

                        rowSkip++;
                    }

                    try
                    {
                        var fileName = $"Product-{Common.GetCurrentTimestamp()}.csv";
                        var filePath = Path.Combine(Product_SourceFolder, fileName);

                        feedService.CreateCSV(dt, Product_SourceFolder, fileName);
                        using (var az = new AzureService())
                        {
                            az.AddBlob(filePath, ProductDestinationContainer, fileName);
                        }

                        messageList.Add(new KeyValuePair<string, string>($"CreateProductFile#{Guid.NewGuid()}", $"New Product File Result"));
                        messageList.Add(new KeyValuePair<string, string>($"CreateProductFile#{Guid.NewGuid()}", Environment.NewLine));
                        messageList.Add(new KeyValuePair<string, string>($"CreateProductFile#{Guid.NewGuid()}", $"New Product File Created @ { filePath} With Product Count: { dt.Rows.Count}"));
                        messageList.Add(new KeyValuePair<string, string>($"CreateProductFile#{Guid.NewGuid()}", Environment.NewLine));
                        messageList.Add(new KeyValuePair<string, string>($"CreateProductFile#{Guid.NewGuid()}", $"New Product Blob With Name: {fileName} Has Been Uploaded To Container: {ProductDestinationContainer}"));


                    }
                    catch (Exception ex)
                    {
                        emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product File Creation");
                    }


                }
            }
            catch (Exception ex)
            {
                emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product File Creation");
            }
            finally
            {
                productFileCreationTimer.Change(pollInterval, pollInterval);
            }
        }
        private static void CreateCategoryFileFile(object state)
        {

            try
            {
                productCategoryFileCreationTimer.Change(Timeout.Infinite, Timeout.Infinite);
                log.Info($"CreateCategoryFileFile load has been ticked @ {DateTime.Now.ToLongTimeString()}");
                string requestUrl = $"{categoryUrl}?email={UserName}&passwd={password}&action=full";
                var data = feedService.DownLoadStringData(requestUrl);
                if (data.Length > 0)
                {
                    DataTable dt = new DataTable();
                    var raw = data.Split('\n');
                    int rowSkip = 1;
                    dt.Columns.Add("CategoryId");
                    dt.Columns.Add("ParentCategoryId");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("Active");
                    DataRow dr;

                    foreach (var item in raw)
                    {
                        if (rowSkip > 2)
                        {
                            var categoryData = item.Split(',');

                            if (categoryData.Length != 3)
                            {
                                continue;
                            }

                            dr = dt.NewRow();
                            try
                            {
                                dr[0] = Common.GetInt(categoryData.GetValue(0).ToString());
                                dr[1] = Common.GetInt(categoryData.GetValue(1).ToString());
                                dr[2] = Common.GetString(categoryData.GetValue(2).ToString());
                                dr[3] = true;
                                dt.Rows.Add(dr);
                            }
                            catch (Exception ex)
                            {
                                emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product Category File Creation");
                            }
                        }
                        rowSkip++;
                    }

                    try
                    {
                        var fileName = $"Category-{Common.GetCurrentTimestamp()}.csv";
                        var filePath = Path.Combine(ProductCategory_SourceFolder, fileName);

                        feedService.CreateCSV(dt, ProductCategory_SourceFolder, fileName);
                        using (var az = new AzureService())
                        {
                            az.AddBlob(filePath, CategoryDestinationContainer, fileName);
                        }

                        messageList.Add(new KeyValuePair<string, string>($"CreateCategoryFileFile#{Guid.NewGuid()}", $"New Product Category File Result"));
                        messageList.Add(new KeyValuePair<string, string>($"CreateCategoryFileFile#{Guid.NewGuid()}", Environment.NewLine));
                        messageList.Add(new KeyValuePair<string, string>($"CreateCategoryFileFile#{Guid.NewGuid()}", $"New Product Category File Created @ { filePath } With Category Count: { dt.Rows.Count}"));
                        messageList.Add(new KeyValuePair<string, string>($"CreateCategoryFileFile#{Guid.NewGuid()}", Environment.NewLine));
                        messageList.Add(new KeyValuePair<string, string>($"CreateCategoryFileFile#{Guid.NewGuid()}", $"New Product Category Blob With Name: {fileName} Has Been Uploaded To Container: {CategoryDestinationContainer}"));

                    }
                    catch (Exception ex)
                    {
                        emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product Category File Creation");
                    }
                }
            }
            catch (Exception ex)
            {
                emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product Category File Creation");
            }
            finally
            {
                productCategoryFileCreationTimer.Change(pollInterval, pollInterval);
            }
        }
        private static void CreateImageFile(object state)
        {
            try
            {
                productImageFileCreationTimer.Change(Timeout.Infinite, Timeout.Infinite);
                log.Info($"CreateImageFile load has been ticked @ {DateTime.Now.ToLongTimeString()}");
                if (File.Exists(Path.Combine(ProductImage_SourceFolder, "image_paths.csv"))) // TO DO : Pull this from some storage like dropbox or cloud or ftp
                {
                    StringBuilder sb = new StringBuilder();
                    using (var reader = new StreamReader(Path.Combine(ProductImage_SourceFolder, "image_paths.csv")))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            sb.AppendLine(line);
                        }
                    }

                    var fileName = $"Image-{Common.GetCurrentTimestamp()}.csv";

                    //Create a file for blob
                    try
                    {
                        File.WriteAllText(Path.Combine(ProductImage_SourceFolder, fileName), sb.ToString());

                    }
                    catch (Exception ex)
                    {
                        emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product Image File Creation");
                    }

                    File.Delete(Path.Combine(ProductImage_SourceFolder, "image_paths.csv"));

                    // Upload blob
                    var filePath = Path.Combine(ProductImage_SourceFolder, fileName);

                    using (var az = new AzureService())
                    {
                        az.AddBlob(filePath, ImageDestinationContainer, fileName);
                    }

                    messageList.Add(new KeyValuePair<string, string>($"CreateImageFile#{Guid.NewGuid()}", $"New Product Image File Result"));
                    messageList.Add(new KeyValuePair<string, string>($"CreateImageFile#{Guid.NewGuid()}", Environment.NewLine));
                    messageList.Add(new KeyValuePair<string, string>($"CreateImageFile#{Guid.NewGuid()}", $"New Product Image File Created @ { filePath}"));
                    messageList.Add(new KeyValuePair<string, string>($"CreateImageFile#{Guid.NewGuid()}", Environment.NewLine));
                    messageList.Add(new KeyValuePair<string, string>($"CreateImageFile#{Guid.NewGuid()}", $"New Product Image Blob With Name: {fileName} Has Been Uploaded To Container: {ImageDestinationContainer}"));
                }
                else
                {
                    log.Info("File not found : image_paths.csv");
                    messageList.Add(new KeyValuePair<string, string>($"CreateImageFile#{Guid.NewGuid()}", $"New Product Image File Result"));
                    messageList.Add(new KeyValuePair<string, string>($"CreateImageFile#{Guid.NewGuid()}", "File not found : image_paths.csv"));
                }

            }
            catch (Exception ex)
            {
                emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product Image File Creation");
            }
            finally
            {
                productImageFileCreationTimer.Change(pollInterval, pollInterval);
            }
        }
        private static void CreateBarcodeFile(object state)
        {
            try
            {
                productBarcodeFileCreationTimer.Change(Timeout.Infinite, Timeout.Infinite);
                log.Info($"CreateBarcodeFile load has been ticked @ {DateTime.Now.ToLongTimeString()}");
                if (File.Exists(Path.Combine(ProductCode_SourceFolder, "barcodes.csv"))) // TO DO : Pull this from some storage like dropbox or cloud or ftp
                {
                    StringBuilder sb = new StringBuilder();
                    using (var reader = new StreamReader(Path.Combine(ProductCode_SourceFolder, "barcodes.csv")))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            sb.AppendLine(line);
                        }
                    }

                    var fileName = $"Barcode-{Common.GetCurrentTimestamp()}.csv";
                    //Create a file for blob
                    try
                    {
                        File.WriteAllText(Path.Combine(ProductCode_SourceFolder, fileName), sb.ToString());
                    }
                    catch (Exception ex)
                    {
                        emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product Code File Creation");
                    }

                    File.Delete(Path.Combine(ProductCode_SourceFolder, "barcodes.csv"));

                    // Upload blob
                    var filePath = Path.Combine(ProductCode_SourceFolder, fileName);

                    using (var az = new AzureService())
                    {
                        az.AddBlob(filePath, BarcodeDestinationContainer, fileName);
                    }
                    

                    messageList.Add(new KeyValuePair<string, string>($"CreateBarcodeFile#{Guid.NewGuid()}", $"New Product Code File Result"));
                    messageList.Add(new KeyValuePair<string, string>($"CreateBarcodeFile#{Guid.NewGuid()}", Environment.NewLine));
                    messageList.Add(new KeyValuePair<string, string>($"CreateBarcodeFile#{Guid.NewGuid()}", $"New Product Code File Created @ { filePath}"));
                    messageList.Add(new KeyValuePair<string, string>($"CreateBarcodeFile#{Guid.NewGuid()}", Environment.NewLine));
                    messageList.Add(new KeyValuePair<string, string>($"CreateBarcodeFile#{Guid.NewGuid()}", $"New Product Code Blob With Name: {fileName} Has Been Uploaded To Container: {BarcodeDestinationContainer}"));

                }
                else
                {
                    log.Info("File not found : barcodes.csv");
                    messageList.Add(new KeyValuePair<string, string>($"CreateBarcodeFile#{Guid.NewGuid()}", $"New Product Code File Result"));
                    messageList.Add(new KeyValuePair<string, string>($"CreateBarcodeFile#{Guid.NewGuid()}", "File not found : barcodes.csv"));
                }

            }
            catch (Exception ex)
            {
                emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product Code File Creation");
            }
            finally
            {
                productBarcodeFileCreationTimer.Change(pollInterval, pollInterval);
            }
        }
        private static void SendNotification(object state)
        {
            try
            {
                notificationTimer.Change(Timeout.Infinite, Timeout.Infinite);
                log.Info($"SendNotification load has been ticked @ {DateTime.Now.ToLongTimeString()}");

                var step1 = messageList.Where(x => x.Key.Contains("CreateCategoryFileFile"));
                var step2 = messageList.Where(x => x.Key.Contains("CreateProductFile"));
                var step3 = messageList.Where(x => x.Key.Contains("CreateImageFile"));
                var step4 = messageList.Where(x => x.Key.Contains("CreateBarcodeFile"));


                if (step1.Count() > 0 && step2.Count() > 0 && step3.Count() > 0 && step4.Count() > 0)
                {
                    emailService.NotifyFileCreation(messageList, "Puck File Creation");
                    messageList = new List<KeyValuePair<string, string>>();
                }

                


            }
            catch (Exception ex)
            {
                emailService.NotifyException(ex.Message, "EXCEPTION: Puck Product Code File Creation");
            }
            finally
            {
                notificationTimer.Change(pollInterval, pollInterval);
            }
        }
        private static void CreateProductImageFile()
        {
            try
            {
                var data = string.Empty;
                var filepath = Path.Combine(ProductImage_SourceFolder, $"Image_paths.csv"); // TO DO : Pull this from some storage like dropbox or cloud or ftp
                using (var reader = new StreamReader(filepath))
                {
                    StringBuilder sb = new StringBuilder();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        sb.AppendLine(line);
                    }

                    data = sb.ToString();
                }
                var raw = data.Split('\n');
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

                        foreach (var image in pi.ImageList)
                        {
                            if (image.FileName != string.Empty)
                            {
                                dbAccess.UpsertProductImage(pi.ProductModel, image.FileName, image.Number, image.IsMain, true);
                            }
                        }
                    }

                    rowSkip++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void LoadProductBarCodeData(bool loadFromUrl)
        {
            try
            {
                var filepath = Path.Combine(productBarcodeFilePath, $"barcodes.csv");

                if (loadFromUrl)
                {
                    filepath = Path.Combine(productBarcodeFilePath, $"Barcode_{Common.GetFileNameWithTimestamp("csv")}");
                    feedService.DownLoadFile(productBarcodeUrl, filepath);
                }

                var data = string.Empty;
                using (var reader = new StreamReader(filepath))
                {
                    StringBuilder sb = new StringBuilder();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        sb.AppendLine(line);
                    }

                    data = sb.ToString();
                }
                var raw = data.Split('\n');
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

                        dbAccess.UpsertProductCode(Obj.ProductModel, Obj.code, true);
                    }

                    rowSkip++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
