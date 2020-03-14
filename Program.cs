using puckatorFeedLoader.BO;
using System;
using System.Collections.Generic;
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

        private static Service myservice = null;

        private static DataAccess dbAccess = null;

        private static string productFilePath = string.Empty;
        private static string productImagePath = string.Empty;
        private static string productBarcodeFilePath = string.Empty;

        static void Main(string[] args)
        {


            LoadMetaData();

            //LoadCategoryData();

            //LoadProductData();

            //LoadProductImagesData();

            LoadProductBarCodeData(true);


        }



        private static void LoadMetaData()
        {
            myservice = new Service();
            dbAccess = new DataAccess();

            UserName = System.Configuration.ConfigurationSettings.AppSettings["FeedLoadUserName"];
            password = System.Configuration.ConfigurationSettings.AppSettings["FeedLoadUserPassword"];
            categoryUrl = System.Configuration.ConfigurationSettings.AppSettings["CategoryDataUrl"];
            productUrl = System.Configuration.ConfigurationSettings.AppSettings["ProductDataUrl"];
            productBarcodeUrl = System.Configuration.ConfigurationSettings.AppSettings["EANDataUrl"];

            productFilePath = System.Configuration.ConfigurationSettings.AppSettings["ProductFilePath"];
            productImagePath = System.Configuration.ConfigurationSettings.AppSettings["ProductImagesFilePath"];
            productBarcodeFilePath = System.Configuration.ConfigurationSettings.AppSettings["ProductBarcodeFilePath"];
            
        }

        private static void LoadProductData()
        {
            try
            {
                string requestUrl = $"{productUrl}?email={UserName}&passwd={password}&action=full";
                var data = myservice.DownLoadStringData(requestUrl);
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
                var data = myservice.DownLoadStringData(requestUrl);
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

        private static void LoadProductImagesData()
        {
            try
            {
                var data = string.Empty;
                using (var reader = new StreamReader(productImagePath))
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

                        ProductImage pi = new ProductImage();
                        pi.ImageList = new List<Image>();

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

        private static void LoadProductBarCodeData(bool mode)
        {
            try
            {
                string requestUrl = productBarcodeUrl;
                var filepath = Path.Combine(productBarcodeFilePath,$"Barcode_{Common.GetFileNameWithTimestamp("csv")}");
                myservice.DownLoadFile(productBarcodeUrl, filepath);

                var data = string.Empty;
                using (var reader = new StreamReader(@"D:\Visual Studio 2019\puckatorFeedLoader\File\EAN\Barcode_14-3-2020-9-1-12.csv"))
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
