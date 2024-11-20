using DemoAutomation.Base;
using DemoAutomation.Pages;
using DemoAutomation.Helpers;
using AventStack.ExtentReports;
using DemoAutomation.Models;


namespace DemoAutomation.Tests
{
    public class LoginTest : BaseTest
    {
        // Relative path for the Users.json file
        private static string usersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests", "TestData", "Users.json");


        [Category("UI Tests")]
        [Test, TestCaseSource(nameof(GetUsers))]
        public void VerifyUserCanLogin(UserModel user)
        {
            // Get the test case name
            string testCaseName = TestContext.CurrentContext.Test.Name;

            // Log the test case name to Extent Reports
            Test.Log(Status.Info, $"Executing test case: {testCaseName}");

            // Initialize the LoginPage
            var loginPage = new LoginPage(Driver, Test);

            Test.Log(Status.Info, "Starting login test.");

            // Capture screenshot before login, including the test case name in the file name
            string beforeLoginScreenshot = DriverHelper.CaptureScreenshot(_screenshotDir, $"{testCaseName}_BeforeLogin");
            Test.Log(Status.Info, "Captured screenshot before login",
                MediaEntityBuilder.CreateScreenCaptureFromPath(beforeLoginScreenshot).Build());

            // Perform login actions
            loginPage.Login(user.Username, user.Password);

            // Capture screenshot after login, including the test case name in the file name
            string afterLoginScreenshot = DriverHelper.CaptureScreenshot(_screenshotDir, $"{testCaseName}_AfterLogin");
            Test.Log(Status.Info, "Captured screenshot after login",
                MediaEntityBuilder.CreateScreenCaptureFromPath(afterLoginScreenshot).Build());

            // Log the verification process
            Test.Log(Status.Info, "Verifying that user successfully logged in.");

            // Use the method from LoginPage to verify the login result
            loginPage.VerifyLoginResult(user.Username);


        }

        // Method to load users from the JSON file and pass them to the test
        public static IEnumerable<TestCaseData> GetUsers()
        {
            // Get users from the JSON file
            var users = UserHelper.GetUsersFromJson(usersFilePath);

            foreach (var user in users)
            {
                // Create a test case and assign a custom test name
                yield return new TestCaseData(user).SetName($"LoginTest_{user.Username}");
            }
        }
    }
}
