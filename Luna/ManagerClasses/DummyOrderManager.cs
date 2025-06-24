using Luna.DataClasses.IDClasses;
using Luna.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luna.ProductSystem;

namespace Luna.ManagerClasses
{
    internal class DummyOrderManager : OrderManager
    {
        List<Order> orders = new List<Order>();
        Action<List<Order>> updateOrdersCallback;

        public override List<Order> GetRecentOrders()
        {
            Console.WriteLine("Getting recent orders...");
            return orders;
        }

        public override void AddOrder(Order order)
        {
            Console.WriteLine("Adding order to list...");
            updateOrders();
        }

        public override void RemoveOrder(OrderID order)
        {
            Console.WriteLine("Removing order from list...");
            updateOrders();
        }

        public override void LoadOrders(IOrderLoader orderLoader)
        {
            Console.WriteLine("Loading recent orders...");
            orders = orderLoader.LoadOrders();
        }

        public override void SetUpdateCallback(Action<List<Order>> updateOrdersCallback)
        {
            this.updateOrdersCallback = updateOrdersCallback;
        }

        private void updateOrders()
        {
            if (updateOrdersCallback == null) return;

            Console.WriteLine("Updating Orders...");
            updateOrdersCallback?.Invoke(orders);
        }
    }
}
