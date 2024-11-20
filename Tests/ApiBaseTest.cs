using RestSharp;
using NUnit.Framework;

namespace DemoAutomation.Base
{
    public class ApiBaseTest:BaseTest
    {
        protected RestClient Client;
        protected RestRequest Request;

        public ApiBaseTest(string baseUrl)
        {
            Client = new RestClient(baseUrl);
            IsApiTest = true; // Marking the test as a DB test
        }

        [SetUp]
        public void ApiSetup()
        {
            Request = new RestRequest();
        }

        // Remove Dispose call for RestClient
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // No need to dispose RestClient here
        }

        protected RestResponse SendRequest(string method, string endpoint, object? body = null)
        {
            Request.Resource = endpoint;
            Request.Method = method.ToUpper() switch
            {
                "GET" => Method.GET,
                "POST" => Method.POST,
                "PUT" => Method.PUT,
                "DELETE" => Method.DELETE,
                _ => throw new ArgumentException($"Unsupported HTTP method: {method}")
            };

            Request.AddHeader("Content-Type", "application/json");

            if (body != null)
            {
                Request.AddJsonBody(body);
            }

            IRestResponse response = Client.Execute(Request);

            // Explicitly cast to RestResponse
            return response as RestResponse;
        }
    }
}
