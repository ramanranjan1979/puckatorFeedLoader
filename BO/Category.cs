using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace puckatorFeedLoader.BO
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int ParentCategoryId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; } 
        
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string Model { get; set; }
        public string EAN { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Dimension { get; set; }

        public string Price { get; set; }

        public string DeliveryCode { get; set; }

        public string Quantity { get; set; }

        public string Categories { get; set; }

        public string Options { get; set; }

        public string MOQ { get; set; }

        public string  ImagesUrl { get; set; }
    }

    public class ProductImage
    {
        public string ProductModel { get; set; }
        public List<Image> ImageList { get; set; }       

    }

    public class Image
    {
        public bool IsMain { get; set; }
        public int Number { get; set; }
        public string FileName { get; set; }
        
    }

    public class Productcode
    {
        public string ProductModel { get; set; }
        public string code { get; set; }

    }
}

