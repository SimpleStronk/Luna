using System;
using System.Collections.Generic;
using System.Linq;
using Luna.DataClasses;
using Luna.DataClasses.IDClasses;
using Luna.ProductSystem;

namespace Luna.ManagerClasses
{
    internal static class ProductManager
    {
        private static Dictionary<ProductID, Product> products;
        static Action<Dictionary<ProductID, Product>> updateProductsCallback;
        static IProductSaver productSaver;

        public static Dictionary<ProductID, Product> GetProducts() {  return products; }

        public static void AddProduct(Product product)
        {
            products.Add(product.GetProductID(), product);
            saveProducts();
            updateProducts();
        }

        public static void RemoveProduct(Product product, bool updateCallback = true)
        {
            RemoveProduct(product.GetProductID(), updateCallback);
        }

        public static void RemoveProduct(ProductID productId, bool updateCallback = true)
        {
            products.Remove(productId);
            saveProducts();

            if (updateCallback) updateProducts();
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

        public static void SetProductSaver(IProductSaver productSaver)
        {
            ProductManager.productSaver = productSaver;
        }

        static void saveProducts()
        {
            productSaver?.OutputProducts(products.Values.ToList());
        }

        static void updateProducts()
        {
            if (updateProductsCallback == null) return;

            Console.WriteLine("Updating Products");
            updateProductsCallback?.Invoke(products);
        }
    }
}
