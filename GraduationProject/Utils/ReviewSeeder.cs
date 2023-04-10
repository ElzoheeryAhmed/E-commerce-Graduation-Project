using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GraduationProject.Utils
{
    public class UserReviewSeeder
    {
        public static List<string> FindMissingUsers()
		{
            // Parsing addedDates.
            string jsonFilePath = Path.Combine(Environment.CurrentDirectory,
                                                "Data",
                                                "InitialSeed",
                                                "users8.json");
            
            var userDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string,object>>>(File.ReadAllText(jsonFilePath));
            List<string> userIds = new List<string>();
            
            foreach (var user in userDict) {
                // Console.WriteLine($"User Number: {user.Key}, User Id: " + userDict[user.Key]["id"]);
                userIds.Add(userDict[user.Key]["id"].ToString());
            }
            
            jsonFilePath = Path.Combine(Environment.CurrentDirectory,
                                        "Data",
                                        "InitialSeed",
                                        "ClothingReviews.json");
            
            var extractedReviews = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(jsonFilePath));
            
            List<string> missingUsers = new List<string>();
            foreach (var key in extractedReviews["reviewerID"]) {
                if (!userIds.Contains(key.Value.ToString())) {
                    missingUsers.Add(key.Value.ToString());
                }
            }
            
            
            jsonFilePath = Path.Combine(Environment.CurrentDirectory,
                                        "Data",
                                        "InitialSeed",
                                        "Home_KitchenReviews.json");

            extractedReviews = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(jsonFilePath));
            foreach (var key in extractedReviews["reviewerID"]) {
                if (!userIds.Contains(key.Value.ToString())) {
                    missingUsers.Add(key.Value.ToString());
                }
            }
            
            
            Dictionary<string, string> missingUsersDict = new Dictionary<string, string>();
            for (int i = 0; i < missingUsers.Count; i++) {
                missingUsersDict.Add(i.ToString(), missingUsers[i]);
            }
            
            // Dump missing users to a json file where the keys are indexes that starts from 0.
            jsonFilePath = Path.Combine(Environment.CurrentDirectory,
                                        "Data",
                                        "InitialSeed",
                                        "missingUsers.json");
            
            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(missingUsersDict));
            
            return new List<string>();
        }
    }
}
