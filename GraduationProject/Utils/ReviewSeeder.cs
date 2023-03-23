using System.Globalization;
using GraduationProject.Models.Dto;
using Newtonsoft.Json;

namespace GraduationProject.Utils
{
    public class ReviewSeeder
    {
        public static List<ReviewDto> Seed(int fileSelector=0)
		{
            string jsonFilePath;
			if (fileSelector == 0)
			{
				jsonFilePath = Path.Combine(Environment.CurrentDirectory,
											"Data",
											"InitialSeed",
											"ClothingReviews.json");
			}
            
            else
            {
                jsonFilePath = Path.Combine(Environment.CurrentDirectory,
											"Data",
											"InitialSeed",
											"Home_KitchenReviews.json");
            }
            
			// Parsing the json file and creating the appropriate objects.
			using (StreamReader r = new StreamReader(jsonFilePath))
			{
				// Parse the JSON into a dynamic object.
				Dictionary<string, Dictionary<string, string>> extractedReviews = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(r.ReadToEnd());

				// Create an empty list to hold all the Reviews.
				List<ReviewDto> reviews = new List<ReviewDto>();

				int counter = 0;
				foreach (var key in extractedReviews["reviewerID"])
				{
					// Parsing the inconsistent reviewDates.
					// DateTime reviewDate = DateTime.ParseExact(extractedReviews["reviewTime"][key.Key], "MM dd, yyyy", CultureInfo.InvariantCulture);
					DateTime reviewDate;
					string[] formats = { "MM d, yyyy", "M d, yyyy", "MM dd, yyyy", "M dd, yyyy" };
					
					DateTime.TryParseExact(extractedReviews["reviewTime"][key.Key],
											formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out reviewDate);
					
					// Creating a Rating object.
					ReviewDto review = new ReviewDto
					{
						UserId = key.Value.ToString(),
						ProductId = extractedReviews["asin"][key.Key].ToString().Trim(),
						ReviewText = extractedReviews["reviewText"][key.Key],
						Timestamp = reviewDate,
					};

					reviews.Add(review);
					Console.WriteLine($"User Id:\t{review.UserId}\nProduct Id:\t{review.ProductId}\nReview Text\t{review.ReviewText.Substring(0, Math.Min(review.ReviewText.Length, 20))}\nTimestamp::\t{review.Timestamp}\n");

					// Below code is for debugging.
					if (counter++ >= 10)
						break;
				}

				return reviews;
			}
		}
    }
}