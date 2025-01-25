using System;

namespace Luna.DataClasses.IDClasses
{
    internal class CustomerID
    {
        int id;
        static int idCount = 0;

        public int GetID()
        {
            return id;
        }

        public static CustomerID CreateSequential()
        {
            CustomerID customerId = new CustomerID();
            customerId.id = idCount++;
            return customerId;
        }
    }
}