﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckFeedFunctionApp.BO
{
    public class Catalogue : Category
    {
        public int ID { get; set; }
        public int Count { get; set; }      

        public List<Catalogue> ChildCatalogue { get; set; }
    }
}