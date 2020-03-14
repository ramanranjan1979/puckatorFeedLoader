using puckatorFeedLoader.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private static Service myservice = null;

        private static DataAccess dbAccess = null;

        static void Main(string[] args)
        {


            LoadMetaData();

            LoadCategoryData();

            LoadProductData();


        }


        private static void LoadMetaData()
        {
            myservice = new Service();
            dbAccess = new DataAccess();

            UserName = System.Configuration.ConfigurationSettings.AppSettings["FeedLoadUserName"];
            password = System.Configuration.ConfigurationSettings.AppSettings["FeedLoadUserPassword"];
            categoryUrl = System.Configuration.ConfigurationSettings.AppSettings["CategoryDataUrl"];
            productUrl = System.Configuration.ConfigurationSettings.AppSettings["ProductDataUrl"];
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
                        return;
                    }


                    var raw = data.Split('\n');
                    int rowSkip = 1;
                    foreach (var item in raw)
                    {
                        if (rowSkip > 2)
                        {
                            var categoryData = item.Split(',');

                            if (categoryData.Length != 13)
                            {
                                return;
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
    }
}
