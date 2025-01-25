using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Luna.DataClasses;
using Luna.DataClasses.IDClasses;
using Luna.ProductSystem;

namespace Luna.ManagerClasses
{
    internal static class ProductManager
    {
        private static Dictionary<ProductID, Product> products;
        static Action<Dictionary<ProductID, Product>> updateProductsCallback;

        public static Dictionary<ProductID, Product> GetProducts() {  return products; }

        public static void AddProduct(Product product)
        {
            products.Add(product.GetProductID(), product);
            updateProducts();
        }
        public static void RemoveProduct(Product product)
        {
            products.Remove(product.GetProductID());
            updateProducts();
        }

        public static Product GetProductByID(ProductID productId)
        {
            return products[productId];
        }

        public static void LoadProducts(IProductLoader productLoader)
        {
            Console.WriteLine("Loading Products...");
            products = productLoader.LoadProducts();
        }

        public static void SetUpdateProductCallback(Action<Dictionary<ProductID, Product>> updateProductsCallback)
        {
            ProductManager.updateProductsCallback = updateProductsCallback;
        }

        static void updateProducts()
        {
            if (updateProductsCallback == null) return;

            Console.WriteLine("Updating Products");
            updateProductsCallback(products);
        }
    }
}
