using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using DemoAutomation.Helpers;

namespace DemoAutomation.Base
{
    public class DBBaseTest : BaseTest
    {
        // Database context options for TestDbContext
        protected DbContextOptions<TestDbContext> DbContextOptions { get; private set; }
        
        // Instance of the database context
        protected TestDbContext DbContext { get; private set; }
        
        // Path to the JSON file containing product data
        private readonly string _jsonFilePath;

        // Instance of ProductHelper to handle product-related operations
        protected ProductHelper ProductHelper { get; private set; }

        public DBBaseTest()
        {
            // Define the path to the product.json file
            string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            _jsonFilePath = Path.Combine(baseDirectory, "Tests","TestData", "product.json");
            IsDbTest = true; // Marking the test as a DB test
        }

        // Setup method called before each test
        [SetUp]
        public void DBSetup()
        {
            // Initialize the in-memory database options
            DbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}") // Unique name for each test
                .Options;

            // Create a new database context instance
            DbContext = new TestDbContext(DbContextOptions);

            // Initialize ProductHelper to help with operations on the ProductModel
            ProductHelper = new ProductHelper(DbContext);

            // Call SeedDatabase to populate the database with initial data from product.json
            SeedDatabase();

            Test.Log(AventStack.ExtentReports.Status.Info, "Database setup complete.");
        }

        // Teardown method called after each test
        [TearDown]
        public void DBTeardown()
        {
            // Dispose the database context to clean up resources
            DbContext.Dispose();

            // Optional: Dispose of ProductHelper if necessary (if it holds resources)
            ProductHelper = null;

            Test.Log(AventStack.ExtentReports.Status.Info, "Database teardown complete.");
        }

        /// <summary>
        /// Seeds the in-memory database with data from product.json.
        /// </summary>
        protected virtual void SeedDatabase()
        {
            if (File.Exists(_jsonFilePath))
            {
                try
                {
                    // Read and deserialize the JSON file containing product data
                    var productJson = File.ReadAllText(_jsonFilePath);
                    var products = JsonConvert.DeserializeObject<List<ProductModel>>(productJson);

                    // Add products to the database using ProductHelper
                    if (products != null && products.Any())
                    {
                        ProductHelper.AddProducts(products); // Use the helper to add products
                        Test.Log(AventStack.ExtentReports.Status.Info, "Database seeded from product.json.");
                    }
                    else
                    {
                        Test.Log(AventStack.ExtentReports.Status.Warning, "No products found in the JSON file.");
                    }
                }
                catch (Exception ex)
                {
                    Test.Log(AventStack.ExtentReports.Status.Error, $"Error seeding database: {ex.Message}");
                }
            }
            else
            {
                Test.Log(AventStack.ExtentReports.Status.Warning, "product.json file not found. Database not seeded.");
            }
        }
    }

    // EF Core DbContext for TestDbContext
    public class TestDbContext : DbContext
    {
        // Constructor to initialize the DbContext with options
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        // DbSet for the ProductModel, this represents the Products table
        public DbSet<ProductModel> Products { get; set; }
    }
}
