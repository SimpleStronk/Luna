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

        /// <summary>
        /// Adds the given object to this object's list of Orders
        /// </summary>
        public override void AddOrder(Order order)
        {
            Console.WriteLine("Adding order to list...");
            updateOrders();
        }

        /// <summary>
        /// Removes the given order from this object's list of orders
        /// </summary>
        public override void RemoveOrder(OrderID order)
        {
            Console.WriteLine("Removing order from list...");
            updateOrders();
        }

        /// <summary>
        /// Loads orders into this object
        /// </summary>
        /// <param name="orderLoader">IOrderLoader object to use to load the orders</param>
        public override void LoadOrders(IOrderLoader orderLoader)
        {
            Console.WriteLine("Loading recent orders...");
            orders = orderLoader.LoadOrders();
        }

        /// <summary>
        /// Calls the given Action when this object's list of orders is updated
        /// </summary>
        /// <param name="updateOrdersCallback">Action to be called</param>
        public override void SetUpdateCallback(Action<List<Order>> updateOrdersCallback)
        {
            this.updateOrdersCallback = updateOrdersCallback;
        }

        /// <summary>
        /// Calls <c>updateOrdersCallback</c> to notify listeners that orders have changed
        /// </summary>
        private void updateOrders()
        {
            Console.WriteLine("Updating Orders...");
            updateOrdersCallback?.Invoke(orders);
        }
    }
}
