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
    internal class SystemManager
    {
        DummyProductLoader productLoader;
        DummyOrderManager orderManager;
        DummyOrderLoader orderLoader;
        DummyOrderSaver orderSaver;
        List<Order> orders;

        public SystemManager()
        {
            productLoader = new DummyProductLoader();
            ProductManager.LoadProducts(productLoader);
            CustomerManager.AddCustomer(new Customer().SetCustomerID(CustomerID.CreateSequential())
                .SetFirstName("Mike").SetlastName("Customer"));

            orderManager = new DummyOrderManager();
            orderLoader = new DummyOrderLoader();
            orderSaver = new DummyOrderSaver();
            orderManager.LoadOrders(orderLoader);
            orderManager.SetUpdateCallback(orderSaver.OutputOrders);
            orders = orderManager.GetRecentOrders();

            foreach (Order order in orders)
            {
                Console.WriteLine($"Order: {order.GetOrderID().ID}, for product {order.GetProductIDs()[0].ID} among other things...");
            }
        }

        public List<Order> GetOrders()
        {
            return orders;
        }
    }
}
