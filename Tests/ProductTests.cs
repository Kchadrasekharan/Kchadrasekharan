using DemoAutomation.Base;

namespace DemoAutomation.Tests
{
    public class ProductTests : DBBaseTest
    {
        // Test for Adding a Product
        [Test]
        public void AddProduct_ShouldAddProductToDatabase()
        {
            // Arrange
            var product = new ProductModel
            {
                Name = "Test Product",
                Price = 29.99,                
                Stock = 100
            };

            // Act
            ProductHelper.AddProducts(new List<ProductModel> { product });

            // Assert
            var addedProduct = DbContext.Products.FirstOrDefault(p => p.Name == "Test Product");
            Assert.That(addedProduct, Is.Not.Null, "Product was not added to the database.");
            Assert.That(addedProduct.Name, Is.EqualTo("Test Product"), "Product name does not match.");
            Assert.That(addedProduct.Price, Is.EqualTo(29.99), "Product price does not match.");
            Assert.That(addedProduct.Stock, Is.EqualTo(100), "Product quantity does not match.");
        }

        // Test for Updating a Product
        [Test]
        public void UpdateProduct_ShouldUpdateProductInDatabase()
        {
            // Arrange
            var existingProduct = new ProductModel
            {
                Name = "Existing Product",
                Price = 19.99,
                Stock = 50
            };

            // Add product to the database
            ProductHelper.AddProducts(new List<ProductModel> { existingProduct });

            // Act: Update the product
            existingProduct.Price = 24.99;
            existingProduct.Stock = 200;
            DbContext.Products.Update(existingProduct);
            DbContext.SaveChanges();

            // Assert
            var updatedProduct = DbContext.Products.FirstOrDefault(p => p.Name == "Existing Product");
            Assert.That(updatedProduct, Is.Not.Null, "Product was not found in the database.");
            Assert.That(updatedProduct.Price, Is.EqualTo(24.99), "Product price was not updated.");
            Assert.That(updatedProduct.Stock, Is.EqualTo(200), "Product quantity was not updated.");
        }

        // Test for Deleting a Product
        [Test]
        public void DeleteProduct_ShouldRemoveProductFromDatabase()
        {
            // Arrange
            var product = new ProductModel
            {
                Name = "Product to Delete",
                Price = 49.99,
                Stock = 30
            };

            // Add product to the database
            ProductHelper.AddProducts(new List<ProductModel> { product });

            // Act: Delete the product
            var productToDelete = DbContext.Products.FirstOrDefault(p => p.Name == "Product to Delete");
            if (productToDelete != null)
            {
                DbContext.Products.Remove(productToDelete);
                DbContext.SaveChanges();
            }

            // Assert
            var deletedProduct = DbContext.Products.FirstOrDefault(p => p.Name == "Product to Delete");
            Assert.That(deletedProduct, Is.Null, "Product was not deleted from the database.");
        }

        // Test for Querying a Product
        [Test]
        public void GetProductByName_ShouldReturnCorrectProduct()
        {
            // Arrange
            var product = new ProductModel
            {
                Name = "Queried Product",
                Price = 15.99,
                Stock = 10
            };

            // Add product to the database
            ProductHelper.AddProducts(new List<ProductModel> { product });

            // Act: Query the product by name
            var queriedProduct = DbContext.Products.FirstOrDefault(p => p.Name == "Queried Product");

            // Assert
            Assert.That(queriedProduct, Is.Not.Null, "Product was not found in the database.");
            Assert.That(queriedProduct.Name, Is.EqualTo("Queried Product"), "Product name does not match.");
            Assert.That(queriedProduct.Price, Is.EqualTo(15.99), "Product price does not match.");
            Assert.That(queriedProduct.Stock, Is.EqualTo(10), "Product quantity does not match.");
        }

        // Test for Product Count in Database
        [Test]
        public void ProductCount_ShouldBeCorrectAfterAddingProducts()
        {
            // Arrange
            var product1 = new ProductModel { Name = "Product 1", Price = 10.99, Stock = 50 };
            var product2 = new ProductModel { Name = "Product 2", Price = 20.99, Stock = 30 };

            // Act: Add products
            ProductHelper.AddProducts(new List<ProductModel> { product1, product2 });

            // Assert: Verify the product count
            var productCount = DbContext.Products.Count();
            Console.Write(productCount);
            Assert.That(productCount, Is.EqualTo(4), "Product count in database is incorrect.");
        }
    }
}
