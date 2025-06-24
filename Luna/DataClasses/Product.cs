using Luna.DataClasses.IDClasses;

namespace Luna.DataClasses
{
    internal class Product
    {
        string productName;
        float productCost;
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

        public float GetCost()
        {
            return productCost;
        }

        public Product SetCost(float productCost)
        {
            this.productCost = productCost;
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
