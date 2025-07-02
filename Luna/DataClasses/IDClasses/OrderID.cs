using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.DataClasses.IDClasses
{
    internal class OrderID
    {
        private int id;

        static int idCount = 0;

        /// <summary>
        /// Internal ID value of this object
        /// </summary>
        public int ID { get { return id; } }

        public static OrderID MakeSequential()
        {
            OrderID orderID = new OrderID();
            orderID.id = ++idCount;
            return orderID;
        }
    }
}
