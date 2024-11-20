using Newtonsoft.Json.Linq;  // Add this to work with JSON
using DemoAutomation.Base;

namespace DemoAutomation.Tests
{
    [TestFixture]
    public class DogApiTests : ApiBaseTest
    {
        // Constructor passing the base URL to the base class
        public DogApiTests() : base("https://dog.ceo/api") // Pass Dog API base URL here
        {
        }

        // Test 1: Get Random Dog Image
        [Test, Category("DogAPI")]
        public void GetRandomDogImage()
        {
            string endpoint = "/breeds/image/random";
            var response = SendRequest("GET", endpoint);

            // Assert the response status code is OK
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Status code is not 200");

            // Assert the response contains 'message' and is a valid image URL
            Assert.That(response.Content.Contains("message"), "Response does not contain 'message'");

            // Use JObject to parse the response and extract the 'message' field
            var jsonResponse = JObject.Parse(response.Content);
            var message = jsonResponse["message"].ToString();

            Assert.That(message, Is.Not.Empty, "Message field is empty");
            Assert.That(message.Contains("https://"), "Response does not contain a valid image URL");
        }

        // Test 2: Get All Breeds
        [Test, Category("DogAPI")]
        public void GetAllBreeds()
        {
            string endpoint = "/breeds/list/all";
            var response = SendRequest("GET", endpoint);

            // Assert the response status code is OK
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Status code is not 200");

            // Assert the response contains 'message' with the list of breeds
            Assert.That(response.Content.Contains("message"), "Response does not contain 'message'");

            // Use JObject to parse the response and extract the list of breeds
            var jsonResponse = JObject.Parse(response.Content);
            var breeds = jsonResponse["message"]; // This is expected to be a JObject containing breeds

            Assert.That(breeds, Is.TypeOf<JObject>(), "Breeds are not in expected format (dictionary)");
        }

        // Test 3: Get Sub-breeds for a Specific Breed (e.g., hound-case sensitive)
        [Test,Category("DogAPI")]
        public void GetSubBreedsForHound()
        {
            string breed = "hound";
            
            string endpoint = $"/breed/{breed}/images";
            var response = SendRequest("GET", endpoint);

            // Assert the response status code is OK
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Status code is not 200");

            // Assert the response contains sub-breeds
            Assert.That(response.Content.Contains("message"), "Response does not contain 'message'");
            Assert.That(response.Content.Contains("afghan"), "Response does not contain 'afghan' sub-breed for hound");

            // Parse the response and check for sub-breeds
            var jsonResponse = JObject.Parse(response.Content);
            var subBreeds = jsonResponse["message"];
            Assert.That(subBreeds, Is.Not.Null, "Sub-breeds are missing");
        }
    }
}
