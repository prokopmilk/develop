using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class SaleModel
    {
        public SaleModel() { }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Tovar> Tovars { get; set; }

    }
}