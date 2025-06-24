using Luna.DataClasses.IDClasses;
using Luna.UI.LayoutSystem;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace Luna.DataClasses
{
    internal class Product
    {
        string productName;
        float productCost;
        Texture2D productIcon;
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

        public Product SetIcon(Texture2D icon)
        {
            productIcon = icon;
            return this;
        }

        public Texture2D GetIcon()
        {
            return productIcon;
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
