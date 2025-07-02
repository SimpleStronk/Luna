using System;

namespace Luna.DataClasses.IDClasses
{
    internal class CustomerID
    {
        int id;
        static int idCount = 0;

        public int GetID()
        {
            CreateSequential();
            return id;
        }

        ///<summary>
        /// Creates a new CustomerID object with the next available internal ID value
        /// </summary>
        public static CustomerID CreateSequential()
        {
            CustomerID customerId = new CustomerID();
            customerId.id = idCount++;
            return customerId;
        }
    }
}