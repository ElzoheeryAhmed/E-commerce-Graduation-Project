using System.Globalization;
using GraduationProject.Models.Dto;
using GraduationProject.Models.ModelEnums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace GraduationProject.Utils
{
	public static class ProductSeeder
	{
		public static List<ProductDto> Seed(string? jsonFilePath)
		{
			if (jsonFilePath == null)
			{
				jsonFilePath = Path.Combine(Environment.CurrentDirectory,
											"Data",
											"InitialSeed",
											"ProductsMetadata.json");
			}
			
			// Parsing the json file and creating the appropriate objects.
			using (StreamReader r = new StreamReader(jsonFilePath))
			{
				// Parse the JSON into a dynamic object.
				dynamic extractedProduts = JsonConvert.DeserializeObject(r.ReadToEnd());
				
				// Create an empty list to hold all the products.
				List<ProductDto> products = new List<ProductDto>();
				
				int counter = 0;
				foreach (var key in extractedProduts.asin)
				{
					// // Parsing the price. If the price is in this format ("2.98 - $3.98"), then we have a price and a discount.
					// // The discount is the difference between the two numbers.
					// string[] parsedPrice = extractedProduts.price[key.Name].ToString().Split('-');
					// decimal price = decimal.Parse(parsedPrice[0].Trim());
					// decimal discount = parsedPrice.Length > 1 ? 0 : decimal.Parse(parsedPrice[0].Trim()) - price;
					Random random = new Random();
					
					
					// Next fields requires extra deserialization.
					string[] categories = JToken.Parse(extractedProduts.category[key.Name].Value.ToString()).ToObject<string[]>();
					string[] descriptions = JToken.Parse(extractedProduts.description[key.Name].Value.ToString()).ToObject<string[]>();
					string[] features = JToken.Parse(extractedProduts.feature[key.Name].Value.ToString()).ToObject<string[]>();
					string[] highResImageURLs = JToken.Parse(extractedProduts.imageURLHighRes[key.Name].Value.ToString()).ToObject<string[]>();
					
					// Parsing addedDates.
					jsonFilePath = Path.Combine(Environment.CurrentDirectory,
											"Data",
											"InitialSeed",
											"product_added_dates.json");
					
					var dateDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonFilePath));
					
					// Creating a Product object.
					ProductDto product = new ProductDto
					{
						Id = key.Value.ToString(),
						Title = extractedProduts.title[key.Name].ToString().Trim(),
						Description = string.Join(". ", descriptions.Where(s => !string.IsNullOrEmpty(s))),
						Price = (decimal)(random.NextDouble() * 900 + 100),
						Discount = 0,
						
                        // Price = price,
						// Discount = discount,
						
                        Status = ProductStatus.Current,
						DateAdded = DateTime.ParseExact(dateDict[key.Value.ToString()], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), // DateTime.Now, // The oldest datetime among the 150794 products is: 971136000 -> 2000-10-10 02:00:00
						Brand = extractedProduts.brand[key.Name].ToString().Trim(),
						VoteCount = int.Parse(extractedProduts.vote_count[key.Name].ToString().Trim()),
						VoteAverage = double.Parse(extractedProduts.vote_average[key.Name].ToString().Trim()),
						// MainCategory = extractedProduts.main_cat[key.Name].ToString().Trim(),
						Categories = string.Join(" || ", categories.Where(s => !string.IsNullOrEmpty(s)).ToList()),
						Features = string.Join(" || ", features.Where(s => !string.IsNullOrEmpty(s)).ToList()),
						HighResImageURLs = string.Join(" || ", highResImageURLs.Where(s => !string.IsNullOrEmpty(s)).ToList()),
					};
					
					products.Add(product);
					
					// Console.WriteLine($"Id:\t\t{product.Id}\nTitle:\t\t{product.Title}\nPrice:\t\t{product.Price}\nDate Added:\t{product.DateAdded}\n");
					
					// Below code is for debugging.
					// if (counter++ >= 10)
					// 	break;
				}
				
				return products;
			}
		}
	}
}


// To revert back to an older migration:
	// Update-Database -Migration:InitialCreate
	// Remove-Migration
	// Add-Migration Product-InitialCreate
	// Update-Database