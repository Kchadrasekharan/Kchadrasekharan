using OpenQA.Selenium;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using DemoAutomation.Helpers;

namespace DemoAutomation.Base
{
    public class BaseTest
    {
        protected IWebDriver Driver => DriverHelper.Driver;
        protected static ExtentReports Extent;
        protected ExtentTest Test;
        protected readonly string _screenshotDir;
        private readonly string _reportDir;
        private static string _reportPath;
        private static BrowserType _browserType;
        private static ScreenSize _screenSize;

        // Flag to check if it's an API test or DBTest (default is false for UI tests)
        protected bool IsApiTest = false;
        protected bool IsDbTest = false;

        public BaseTest()
        {
            string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            _screenshotDir = Path.Combine(baseDirectory, "Screenshots");
            _reportDir = Path.Combine(baseDirectory, "Reports");

            // Ensure screenshot and report directories exist if not these will be created
            Directory.CreateDirectory(_screenshotDir);
            Directory.CreateDirectory(_reportDir);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            if (Extent == null)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                _reportPath = Path.Combine(_reportDir, $"ExtentReport_{timestamp}.html");

                // Set the global browser and screen size configurations 
                _browserType = BrowserType.Chrome;
                _screenSize = ScreenSize.Desktop;

                var sparkReporter = new ExtentSparkReporter(_reportPath)
                {
                    Config =
                    {
                        DocumentTitle = "Automation Test Report",
                        ReportName = "Demo for Stellar Science",
                        Theme = AventStack.ExtentReports.Reporter.Config.Theme.Standard
                    }
                };

                Extent = new ExtentReports();
                Extent.AttachReporter(sparkReporter);

                Extent.AddSystemInfo("Environment", "Live");
                Extent.AddSystemInfo("User", "SDET");
                Extent.AddSystemInfo("Browser", _browserType.ToString());
                Extent.AddSystemInfo("Screen Size", _screenSize.ToString());
                Console.WriteLine("ExtentReports with SparkReporter Initialized.");
            }
        }

        [SetUp]
        public void Setup()
        {
            if (!IsApiTest && !IsDbTest)
            {
                // Initialize the driver with browser type and screen size (only for UI tests)
                DriverHelper.InitializeDriver(_browserType, _screenSize);

                // Navigate to the base URL (only for UI tests)
                DriverHelper.GoToUrl("https://www.saucedemo.com/"); // Replace with Base URL
            }

            // Initialize the ExtentTest for all tests (UI or API)
            Test = Extent.CreateTest(TestContext.CurrentContext.Test.Name);
            Test.Log(Status.Info, "Test Setup: Starting test - " + TestContext.CurrentContext.Test.Name);

            // Dynamically assign categories to the test in ExtentReports
            AssignCategoriesToTest(Test);

            if (IsApiTest)
            {
                Test.AssignCategory("ApiTests");
            }

            if (IsDbTest)
            {
                Test.AssignCategory("Database Tests");
            }
        }

        private void AssignCategoriesToTest(ExtentTest test)
        {
            // Get the current test method from TestContext
            var methodInfo = TestContext.CurrentContext.Test.Method;

            // Get the Category attributes applied to the method
            var categoryAttributes = methodInfo.GetCustomAttributes<CategoryAttribute>(false)
                                               .Select(cat => cat.Name)
                                               .ToList();

            // Log to verify the retrieved categories
            Console.WriteLine("Categories: " + string.Join(", ", categoryAttributes));

            // Assign categories to the ExtentTest instance
            foreach (var category in categoryAttributes)
            {
                test.AssignCategory(category);
            }
        }

        [TearDown]
        public void TearDown()
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            var stackTrace = TestContext.CurrentContext.Result.StackTrace ?? "";

            // Handle the tear down logic differently for DB, UI and API tests
            if (IsDbTest)
            {
                // Log database test results (no WebDriver or screenshots)
                if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    Test.Fail("Database Test Failed. Stack Trace: " + stackTrace);
                }
                else if (testStatus == NUnit.Framework.Interfaces.TestStatus.Passed)
                {
                    Test.Pass("Database Test Passed");
                }
                else
                {
                    Test.Skip("Database Test Skipped");
                }
            }
            else if (IsApiTest)
            {
                // Log the result without WebDriver and screenshot for API tests
                if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    Test.Fail("Test Failed. Stack Trace: " + stackTrace);
                }
                else if (testStatus == NUnit.Framework.Interfaces.TestStatus.Passed)
                {
                    Test.Pass("Test Passed");
                }
                else
                {
                    Test.Skip("Test Skipped");
                }
            }
            else
            {
                // Capture screenshot and log results for UI tests only
                if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    string screenshotPath = DriverHelper.CaptureScreenshot(_screenshotDir, TestContext.CurrentContext.Test.Name);
                    Test.Fail("Test Failed", MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
                    Test.Log(Status.Fail, "Test Failed. Stack Trace: " + stackTrace);
                }
                else if (testStatus == NUnit.Framework.Interfaces.TestStatus.Passed)
                {
                    string screenshotPath = DriverHelper.CaptureScreenshot(_screenshotDir, TestContext.CurrentContext.Test.Name);
                    Test.Pass("Test Passed", MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
                    Test.Log(Status.Pass, "Test Passed");
                }
                else
                {
                    Test.Log(Status.Skip, "Test Skipped");
                }

                DriverHelper.CleanupDriver();
                Test.Log(Status.Info, "WebDriver closed.");
            }


        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Extent.Flush();
        }
    }
}
