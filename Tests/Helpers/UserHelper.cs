using Newtonsoft.Json;
using DemoAutomation.Models;

namespace DemoAutomation.Helpers
{
    public static class UserHelper
    {
        public static List<UserModel> GetUsersFromJson(string filePath)
        {
            // Read the JSON file
            string json = File.ReadAllText(filePath);
            
            // Deserialize the JSON to a list of UserModel objects
            List<UserModel> users = JsonConvert.DeserializeObject<List<UserModel>>(json);
            return users;
        }
    }
}
