using OpenQA.Selenium;
using AventStack.ExtentReports;


namespace DemoAutomation.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver Driver;
        private readonly ExtentTest Test;

        // Constructor to initialize the page with Driver and Test (for logging)
        public LoginPage(IWebDriver driver, ExtentTest test)
        {
            Driver = driver;
            Test = test;
        }

        // Define locators for login fields
        private IWebElement UsernameField => Driver.FindElement(By.Id("user-name"));
        private IWebElement PasswordField => Driver.FindElement(By.Id("password"));
        private IWebElement LoginButton => Driver.FindElement(By.Id("login-button"));

        // Method to log in, with logging at each step
        public void Login(string username, string password)
        {
            Test.Log(Status.Info, "Entering username.");
            UsernameField.SendKeys(username);

            Test.Log(Status.Info, "Entering password.");
            PasswordField.SendKeys(password);

            Test.Log(Status.Info, "Clicking on Login button.");
            LoginButton.Click();
        }
        // Method to verify login result based on the username
        public void VerifyLoginResult(string username)
        {
            try
            {
                if (username == "standard_user")
                {
                    Assert.That(Driver.Title.Contains("Swag Labs"), "Standard User login failed.");
                    Assert.That(IsProductPageDisplayed(), "Standard User - Product page is not displayed.");
                }
                else if (username == "locked_out_user")
                {
                    Assert.That(Driver.Title.Contains("Swag Labs"), "Locked Out User login failed.");
                    Assert.That(IsLockedOutPageDisplayed(), "Locked Out User - Locked Out page is not displayed.");
                }
                else if (username == "problem_user")
                {
                    Assert.That(Driver.Title.Contains("Swag Labs"), "Problem User login failed.");
                    Assert.That(IsProblemPageDisplayed(), "Problem User - Problem page is not displayed.");
                }
                else
                {
                    Assert.That(Driver.Title.Contains("Swag Labs"), "Unknown User login failed.");

                }
            }
            catch (Exception ex)
            {
                Test.Info("VerifyLoginResult Exception" + ex.Message);
            }
        }

        // Methods to check if specific pages are displayed
        public bool IsProductPageDisplayed()
        {
            return Driver.FindElement(By.ClassName("inventory_list")).Displayed;
        }

        public bool IsLockedOutPageDisplayed()
        {
            return Driver.FindElement(By.ClassName("error-message-container")).Displayed;
        }

        public bool IsProblemPageDisplayed()
        {
            return Driver.FindElement(By.ClassName("error-message-container")).Displayed;
        }
    }
}
