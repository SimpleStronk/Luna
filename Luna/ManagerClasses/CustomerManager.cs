using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Luna.DataClasses;
using Luna.DataClasses.IDClasses;

namespace Luna.ManagerClasses
{
    internal class CustomerManager
    {
        private static Dictionary<CustomerID, Customer> customers = new Dictionary<CustomerID, Customer>();

        public static Dictionary<CustomerID, Customer> GetCustomers()
        {
            return customers;
        }

        public static Customer GetCustomerByID(CustomerID customerId)
        {
            return customers[customerId];
        }

        public static void AddCustomer(Customer customer)
        {
            customers.Add(customer.GetCustomerID(), customer);
        }
    }
}