using Luna.DataClasses;
using Luna.DataClasses.IDClasses;
using Luna.ProductSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.ManagerClasses
{
    internal abstract class OrderManager
    {
        public abstract List<Order> GetRecentOrders();
        public abstract void AddOrder(Order order);
        public abstract void RemoveOrder(OrderID orderID);
        public abstract void LoadOrders(IOrderLoader orderLoader);
        public abstract void SetUpdateCallback(Action<List<Order>> updateCallback);
    }
}
