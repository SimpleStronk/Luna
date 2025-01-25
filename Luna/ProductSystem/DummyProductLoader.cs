using Luna.DataClasses;
using Luna.DataClasses.IDClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.ProductSystem
{
    internal class DummyProductLoader : IProductLoader
    {
        public Dictionary<ProductID, Product> LoadProducts()
        {
            Console.WriteLine("Product Loader doing its thing...");

            Dictionary<ProductID, Product> products = new Dictionary<ProductID, Product>();
            Product product = new Product().SetProductID(ProductID.CreateSequential()).SetName("Jordan's Lemma");
            products.Add(product.GetProductID(), product);
            return products;
        }
    }
}
