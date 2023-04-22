using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbApp
{
    public class Product
    {
        public string id { get; set; }
        public string name { get; set; }
        public string categoryId { get; set; }
        public string price { get; set; }
        public string[] tags { get; set; }
    }
}
