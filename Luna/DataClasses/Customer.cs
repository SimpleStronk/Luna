using System;
using Luna.DataClasses.IDClasses;

namespace Luna.DataClasses
{
    internal class Customer
    {
        private CustomerID customerId;
        private string firstName;
        private string lastName;

        public CustomerID GetCustomerID()
        {
            return customerId;
        }

        public Customer SetCustomerID(CustomerID customerId)
        {
            this.customerId = customerId;
            return this;
        }

        /// <summary>
        /// The full name associated with this Customer object
        /// </summary>
        public string FullName
        {
            get
            {
                return $"{firstName} {lastName}";
            }
        }

        /// <summary>
        /// The first name associated with this customer object
        /// </summary>
        public string FirstName
        {
            get
            {
                return firstName;
            }
        }

        /// <summary>
        /// The last name associated with this Customer object
        /// </summary>
        public string LastName
        {
            get
            {
                return lastName;
            }
        }

        public Customer SetFirstName(string firstName)
        {
            this.firstName = firstName;
            return this;
        }

        public Customer SetlastName(string lastName)
        {
            this.lastName = lastName;
            return this;
        }
    }
}