using Luna.DataClasses;
using Luna.DataClasses.IDClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.ProductSystem
{
    internal interface IProductLoader
    {
        public Dictionary<ProductID, Product> LoadProducts();
    }
}
