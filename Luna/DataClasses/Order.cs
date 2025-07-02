using Luna.DataClasses.IDClasses;
using System;
using System.Collections.Generic;

namespace Luna.DataClasses
{
    internal class Order
    {
        private OrderID orderID;
        private List<ProductID> productIDs;
        private LunaDateTime orderPlacedDate;
        private LunaDateTime orderResolvedDate;
        public enum OrderStatus { Null, Ordered, Completed, Cancelled, Returning, Returned };
        private OrderStatus orderStatus = OrderStatus.Null;

        private CustomerID customerId;

        /// <summary>
        /// Creates a new Order object, with a unique OrderID value
        /// </summary>
        public Order()
        {
            orderID = OrderID.MakeSequential();
        }

        #region getters_setters
        public OrderID GetOrderID()
        {
            return orderID;
        }

        public List<ProductID> GetProductIDs()
        {
            return productIDs;
        }

        public Order SetProductIDs(List<ProductID> productIDs)
        {
            this.productIDs = productIDs;
            return this;
        }

        public LunaDateTime GetOrderPlacedDate()
        {
            return orderPlacedDate;
        }

        public Order SetOrderPlacedDate(LunaDateTime dateTime)
        {
            orderPlacedDate = dateTime;
            return this;
        }

        public OrderStatus GetOrderStatus()
        {
            return orderStatus;
        }

        public Order SetOrderStatus(OrderStatus orderStatus)
        {
            this.orderStatus = orderStatus;
            return this;
        }

        public Order SetCustomerID(CustomerID customerId)
        {
            this.customerId = customerId;
            return this;
        }

        public CustomerID GetCustomerID()
        {
            return customerId;
        }
        #endregion
    }
}
