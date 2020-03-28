using puckatorFeedLoader.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace puckatorFeedLoader
{
    class Program
    {
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

        private static string sourceFolder = string.Empty;
        private static string ProductDestinationContainer = string.Empty;

        static void Main(string[] args)
        {


            LoadMetaData();

            //LoadCategoryData();

            //LoadProductData();

            //LoadProductImagesData(false);

            //LoadProductBarCodeData(false);

            //RefreshCatalogue();

            CreateProductFile();


        }

        private static void RefreshCatalogue(int parentCategory = 0)
        {
            var data = dbAccess.GetCategoryByParentCategoryId(parentCategory);

            foreach (System.Data.DataRow d in data.Tables[0].Rows)
            {
                var cat = new Catalogue()
                {
                    ID = int.Parse(d.ItemArray[0].ToString()),
                    CategoryId = int.Parse(d.ItemArray[1].ToString()),
                    ParentCategoryId = int.Parse(d.ItemArray[2].ToString()),
                    Description = d.ItemArray[3].ToString()
                };

                myCatalogue.Add(cat);

                if (dbAccess.GetCategoryByParentCategoryId(cat.CategoryId).Tables[0].Rows.Count > 0)
                {
                    cat.ChildCatalogue = new List<Catalogue>
                    {
                        AddCatalogue(cat.CategoryId)
                    };
                    RefreshCatalogue(cat.CategoryId);
                }

                catalogueLevel++;
            }
        }

        private static Catalogue AddCatalogue(int categoryId)
        {
            return new Catalogue() { CategoryId = categoryId };
        }

        private static void LoadMetaData()
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

            sourceFolder = System.Configuration.ConfigurationManager.AppSettings["SourceFolder"];
            ProductDestinationContainer = System.Configuration.ConfigurationManager.AppSettings["ProductDestinationContainer"];

            myCatalogue = new List<Catalogue>();

        }

        private static void LoadProductData()
        {
            try
            {
                string requestUrl = $"{productUrl}?email={UserName}&passwd={password}&action=full";
                var data = feedService.DownLoadStringData(requestUrl);
                if (data.Length > 0)
                {
                    if (data.ToUpper() == "<br>Dropship Feed Error: You must wait 2 hours between product feed requests.".ToUpper())
                    {
                        using (var reader = new StreamReader(productFilePath))
                        {
                            StringBuilder sb = new StringBuilder();
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                sb.AppendLine(line);
                            }

                            data = sb.ToString();
                        }
                    }


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

                            dbAccess.UpsertProduct(Obj.ProductId, Obj.Model, Obj.EAN, Obj.Name, Obj.Description, Obj.Dimension, Obj.Price, Obj.DeliveryCode, Obj.Quantity, Obj.Categories, Obj.Options, Obj.MOQ, Obj.ImagesUrl);
                        }

                        rowSkip++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void LoadCategoryData()
        {
            try
            {
                string requestUrl = $"{categoryUrl}?email={UserName}&passwd={password}&action=full";
                var data = feedService.DownLoadStringData(requestUrl);
                if (data.Length > 0)
                {
                    var raw = data.Split('\n');
                    int rowSkip = 1;
                    foreach (var item in raw)
                    {
                        if (rowSkip > 2)
                        {
                            var categoryData = item.Split(',');

                            if (categoryData.Length != 3)
                            {
                                return;
                            }

                            Category catObj = new Category()
                            {
                                CategoryId = Common.GetInt(categoryData.GetValue(0).ToString()),
                                ParentCategoryId = Common.GetInt(categoryData.GetValue(1).ToString()),
                                Description = Common.GetString(categoryData.GetValue(2).ToString()),
                                Active = true
                            };

                            dbAccess.UpsertCategory(catObj.CategoryId, catObj.ParentCategoryId, catObj.Description, catObj.Active);
                        }

                        rowSkip++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void LoadProductImagesData(bool loadFromUrl)
        {
            try
            {
                var data = string.Empty;
                var filepath = Path.Combine(productImagePath, $"Image_paths.csv");

                if (loadFromUrl)
                {
                    filepath = Path.Combine(productImagePath, $"Images_{Common.GetFileNameWithTimestamp("csv")}");
                    feedService.DownLoadFile(productImageUrl, filepath);
                }

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

        private static void CreateProductFile()
        {
            try
            {
                string requestUrl = $"{productUrl}?email={UserName}&passwd={password}&action=full";
                //var data = "<br>Dropship Feed Error: You must wait 2 hours between product feed requests.";
                var data = feedService.DownLoadStringData(requestUrl);
                if (data.Length > 0)
                {
                    if (data.ToUpper() == "<br>Dropship Feed Error: You must wait 2 hours between product feed requests.".ToUpper())
                    {
                        var messageList = new List<string>();
                        messageList.Add(data);
                        emailService.NotifyProductFileCreation(messageList);
                        return;

                        //using (var reader = new StreamReader(@"D:\GIT\puckatorFeedLoader\File\Product\Product-202003281601363691.csv"))
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
                            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
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
                        var filePath = Path.Combine(sourceFolder, fileName);

                        feedService.CreateCSV(dt, sourceFolder, fileName);
                        using (var az = new AzureService())
                        {
                            az.AddBlob(filePath, ProductDestinationContainer, fileName);
                        }

                        var messageList = new List<string>();
                        messageList.Add($"New Product File Created @ { filePath} With Product Count: { dt.Rows.Count}");
                        messageList.Add(Environment.NewLine);
                        messageList.Add($"New Product Blob With Name: {fileName} Has Been Uploaded To Container: {ProductDestinationContainer}");

                        emailService.NotifyProductFileCreation(messageList);
                    }
                    catch (Exception ex)
                    {
                        emailService.NotifyException(ex.Message);
                    }


                }
            }
            catch (Exception ex)
            {
                emailService.NotifyException(ex.Message);
            }
        }


    }
}
