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

        public string FullName
        {
            get
            {
                return $"{firstName} {lastName}";
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }
        }

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