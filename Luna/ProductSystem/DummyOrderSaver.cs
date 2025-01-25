﻿using Luna.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.ProductSystem
{
    internal class DummyOrderSaver : IOrderSaver
    {
        public void OutputOrders(List<Order> orders)
        {
            Console.WriteLine("Outputting Orders...");
        }
    }
}
