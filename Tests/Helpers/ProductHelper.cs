using DemoAutomation.Base;

namespace DemoAutomation.Helpers
{
    public class ProductHelper
    {
        private readonly TestDbContext _dbContext;

        // Constructor to initialize the ProductHelper with a DbContext instance
        public ProductHelper(TestDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // Method to add a list of products to the database
        public void AddProducts(List<ProductModel> products)
        {
            if (products == null || !products.Any())
            {
                throw new ArgumentException("Products list cannot be null or empty.", nameof(products));
            }

            // Add the products to the DbContext
            _dbContext.Products.AddRange(products);
            _dbContext.SaveChanges(); // Commit the changes to the database
        }

        // Method to retrieve all products from the database
        public List<ProductModel> GetAllProducts()
        {
            return _dbContext.Products.ToList(); // Return all products as a list
        }

        // Method to retrieve a product by its ID
        public ProductModel GetProductById(int id)
        {
            return _dbContext.Products.FirstOrDefault(p => p.Id == id); // Return the product or null if not found
        }

        // Method to update an existing product's information
        public void UpdateProduct(ProductModel product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            }

            var existingProduct = _dbContext.Products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct == null)
            {
                throw new InvalidOperationException($"Product with ID {product.Id} does not exist.");
            }

            // Update the properties of the existing product
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;

            _dbContext.SaveChanges(); // Commit the changes to the database
        }

        // Method to delete a product by its ID
        public void DeleteProduct(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {id} does not exist.");
            }

            _dbContext.Products.Remove(product); // Remove the product from the DbContext
            _dbContext.SaveChanges(); // Commit the changes to the database
        }

        // Method to check if a product exists by its ID
        public bool ProductExists(int id)
        {
            return _dbContext.Products.Any(p => p.Id == id); // Check if the product exists
        }
    }
}
