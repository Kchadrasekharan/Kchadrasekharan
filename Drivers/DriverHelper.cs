using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Drawing;

namespace DemoAutomation.Helpers
{
    public static class DriverHelper
    {
        private static IWebDriver _driver;
        private static WebDriverWait _wait;

        // Static method to navigate to a URL
        public static void GoToUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentException("URL cannot be null or empty.", nameof(url));

                Console.WriteLine($"Navigating to URL: {url}");
                _driver?.Navigate().GoToUrl(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to URL: {ex.Message}");
                throw;
            }
        }

        // Static method to capture a screenshot
        public static string CaptureScreenshot(string screenshotDir, string testName)
        {
            try
            {
                if (_driver == null)
                    throw new InvalidOperationException("Driver is not initialized. Cannot capture screenshot.");

                if (!Directory.Exists(screenshotDir))
                {
                    Console.WriteLine($"Screenshot directory does not exist. Creating: {screenshotDir}");
                    Directory.CreateDirectory(screenshotDir);
                }

                string screenshotPath = Path.Combine(screenshotDir, $"{testName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
                Screenshot screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                screenshot.SaveAsFile(screenshotPath);
                Console.WriteLine($"Screenshot saved at: {screenshotPath}");
                return screenshotPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing screenshot: {ex.Message}");
                throw;
            }
        }

        // Static property to access the driver
        public static IWebDriver Driver
        {
            get
            {
                if (_driver == null)
                    throw new InvalidOperationException("Driver is not initialized. Please call InitializeDriver first.");
                return _driver;
            }
        }

        // Initialize the WebDriver based on browser type and screen size
        public static void InitializeDriver(BrowserType browser = BrowserType.Chrome, ScreenSize screenSize = ScreenSize.Desktop, int maxRetries = 3)
        {
            int attempts = 0;

            while (attempts < maxRetries)
            {
                try
                {
                    Console.WriteLine($"Initializing WebDriver. Attempt {attempts + 1}/{maxRetries}");
                    _driver = CreateDriver(browser);
                    _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30)); // Initialize WebDriverWait
                    SetScreenSize(screenSize);
                    Console.WriteLine("WebDriver initialized successfully.");
                    return;
                }
                catch (Exception ex)
                {
                    attempts++;
                    Console.WriteLine($"Error initializing WebDriver: {ex.Message}");
                    if (attempts >= maxRetries)
                        throw new Exception("Failed to initialize WebDriver after maximum retries.", ex);

                    Console.WriteLine("Retrying WebDriver initialization...");
                }
            }
        }

        // Create the WebDriver based on the browser type
        private static IWebDriver CreateDriver(BrowserType browser)
        {
            try
            {
                return browser switch
                {
                    BrowserType.Chrome => new ChromeDriver(),
                    BrowserType.Firefox => new FirefoxDriver(),
                    BrowserType.Edge => new EdgeDriver(),
                    BrowserType.Safari => new SafariDriver(),
                    _ => throw new NotSupportedException($"Unsupported browser: {browser}")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating WebDriver for {browser}: {ex.Message}");
                throw;
            }
        }

        // Set the window size based on the screen size
        private static void SetScreenSize(ScreenSize screenSize)
        {
            try
            {
                Console.WriteLine($"Setting screen size to: {screenSize}");
                switch (screenSize)
                {
                    case ScreenSize.Desktop:
                        _driver.Manage().Window.Maximize();
                        break;
                    case ScreenSize.Tablet:
                        _driver.Manage().Window.Size = new Size(768, 1024); // Tablet size
                        break;
                    case ScreenSize.Mobile:
                        _driver.Manage().Window.Size = new Size(375, 667); // Mobile size
                        break;
                    default:
                        _driver.Manage().Window.Maximize();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting screen size: {ex.Message}");
                throw;
            }
        }

        // Cleanup and close the driver
        public static void CleanupDriver()
        {
            try
            {
                Console.WriteLine("Cleaning up WebDriver...");
                _driver?.Quit();
                _driver = null;
                Console.WriteLine("WebDriver cleanup completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during WebDriver cleanup: {ex.Message}");
            }
        }

        // Static method to wait for an element to be visible
        public static IWebElement WaitForElementToBeVisible(By by, int timeoutInSeconds = 30)
        {
            try
            {
                Console.WriteLine($"Waiting for element to be visible: {by}");
                return _wait.Until(ExpectedConditions.ElementIsVisible(by));
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"Element not visible within {timeoutInSeconds} seconds: {by}");
                throw new NoSuchElementException($"Element {by} was not visible within {timeoutInSeconds} seconds.", ex);
            }
        }

        // Static method to wait for an element to be clickable
        public static IWebElement WaitForElementToBeClickable(By by, int timeoutInSeconds = 30)
        {
            try
            {
                Console.WriteLine($"Waiting for element to be clickable: {by}");
                return _wait.Until(ExpectedConditions.ElementToBeClickable(by));
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"Element not clickable within {timeoutInSeconds} seconds: {by}");
                throw new ElementNotInteractableException($"Element {by} was not clickable within {timeoutInSeconds} seconds.", ex);
            }
        }

        // Static method to wait for page load
        public static void WaitForPageLoad()
        {
            try
            {
                Console.WriteLine("Waiting for page to load...");
                _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").ToString() == "complete");
                Console.WriteLine("Page loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error waiting for page load: {ex.Message}");
                throw;
            }
        }
    }

    // Enum for Browser types
    public enum BrowserType
    {
        Chrome,
        Firefox,
        Edge,
        Safari
    }

    // Enum for Screen sizes
    public enum ScreenSize
    {
        Desktop,
        Tablet,
        Mobile
    }
}
