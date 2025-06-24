using Luna.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.ProductSystem
{
    internal class DummyProductSaver : IProductSaver
    {
        public void OutputProducts(List<Product> products)
        {
            Console.WriteLine("Outputting Products...");
        }
    }
}
