using Luna.DataClasses;
using Luna.DataClasses.IDClasses;
using Luna.ManagerClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.ProductSystem
{
    internal class DummyOrderLoader : IOrderLoader
    {
        public List<Order> LoadOrders()
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order()
                .SetProductIDs(new List<ProductID> { ProductManager.GetProducts().First().Key })
                .SetOrderPlacedDate(LunaDateTime.Now)
                .SetOrderStatus(Order.OrderStatus.Returned)
                .SetCustomerID(CustomerManager.GetCustomers().First().Key));
            return orders;
        }
    }
}
