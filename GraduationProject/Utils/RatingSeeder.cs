
using GraduationProject.Models.Dto;
using Newtonsoft.Json;

namespace GraduationProject.Utils
{
    public class RatingSeeder
    {
        public static List<RatingDto> Seed(string? jsonFilePath)
		{
			if (jsonFilePath == null)
			{
				jsonFilePath = Path.Combine(Environment.CurrentDirectory,
											"Data",
											"InitialSeed",
											"ProductsRatings.json");
			}

			// Parsing the json file and creating the appropriate objects.
			using (StreamReader r = new StreamReader(jsonFilePath))
			{
				// Parse the JSON into a dynamic object.
				Dictionary<string, Dictionary<string, object>> extractedRatings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(r.ReadToEnd());

				// Create an empty list to hold all the Ratings.
				List<RatingDto> ratings = new List<RatingDto>();

				int counter = 0;
                Random random = new Random();
				foreach (var key in extractedRatings["userID"])
				{
					// Creating a Rating object.
					RatingDto rating = new RatingDto
					{
						UserId = key.Value.ToString(),
						ProductId = extractedRatings["itemID"][key.Key].ToString().Trim(),
						RatingValue = Convert.ToInt32(extractedRatings["rating"][key.Key]),
						Timestamp = DateTimeOffset.FromUnixTimeSeconds((long) extractedRatings["timestamp"][key.Key]).DateTime,
					};

					ratings.Add(rating);
					
					Console.WriteLine($"User Id:\t{rating.UserId}\nProduct Id:\t{rating.ProductId}\nRating Value\t{rating.RatingValue}\nTimestamp::\t{rating.Timestamp}\n");

					// Below code is for debugging.
					if (counter++ >= 10)
						break;
				}

				return ratings;
			}
		}
    }
}