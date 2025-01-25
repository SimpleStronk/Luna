using Luna.DataClasses.IDClasses;

namespace Luna.DataClasses
{
    internal class Product
    {
        string productName;
        ProductID productID;

        #region getters_setters
        public string GetName()
        {
            return productName;
        }

        public Product SetName(string productName)
        {
            this.productName = productName;
            return this;
        }

        public ProductID GetProductID()
        {
            return productID;
        }

        public Product SetProductID(ProductID productID)
        {
            this.productID = productID;
            return this;
        }
        #endregion
    }
}
