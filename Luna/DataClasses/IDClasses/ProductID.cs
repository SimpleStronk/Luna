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

        /// <summary>
        /// Internal ID value of this object
        /// </summary>
        public int ID { get { return id; } }

        /// <summary>
        /// Creates a new ProductID object with the next available internal ID value
        /// </summary>
        public static ProductID CreateSequential()
        {
            ProductID productID = new ProductID();
            productID.id = ++idCount;
            return productID;
        }

        /// <summary>
        /// Creates a new ProductID object with the given ID value
        /// </summary>
        /// <param name="index">value to be used as the object's internal ID value</param>
        public static ProductID FromIndex(int index)
        {
            ProductID productID = new ProductID();
            productID.id = index;
            return productID;
        }
    }
}
