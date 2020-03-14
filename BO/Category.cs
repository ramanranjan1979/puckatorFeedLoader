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

}

