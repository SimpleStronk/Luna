using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Luna.DataClasses.IDClasses
{
    internal class ProductID
    {
        private int id;

        static int idCount = 0;

        public int ID { get { return id; } }

        public static ProductID CreateSequential()
        {
            ProductID productID = new ProductID();
            productID.id = ++idCount;
            return productID;
        }

        public static ProductID FromIndex(int index)
        {
            ProductID productID = new ProductID();
            productID.id = index;
            return productID;
        }
    }
}
