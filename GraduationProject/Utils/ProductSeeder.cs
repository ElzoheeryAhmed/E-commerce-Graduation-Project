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
				dynamic extractedProducts = JsonConvert.DeserializeObject(r.ReadToEnd());
				
				// Create an empty list to hold all the products.
				List<ProductDto> products = new List<ProductDto>();
				
				foreach (var key in extractedProducts.asin)
				{
					// // Parsing the price. If the price is in this format ("2.98 - $3.98"), then we have a price and a discount.
					// // The discount is the difference between the two numbers.
					// string[] parsedPrice = extractedProduts.price[key.Name].ToString().Split('-');
					// decimal price = decimal.Parse(parsedPrice[0].Trim());
					// decimal discount = parsedPrice.Length > 1 ? 0 : decimal.Parse(parsedPrice[0].Trim()) - price;
					Random random = new Random();
					
					
					// Next fields requires extra deserialization.
					string[] categories = JToken.Parse(extractedProducts.category[key.Name].Value.ToString()).ToObject<string[]>();
					
					// Parsing if the description does not contain any nulls: string[] descriptions = JToken.Parse(extractedProducts.description[key.Name].Value.ToString()).ToObject<string[]>();
					// Description can take null values in the dataset, thus we need to be extra careful when parsing it.
					string description = extractedProducts.description[key.Name].Value;
					if (description != null) {
						try {
							description = string.Join("", JToken.Parse(description).ToObject<string[]>()
												.Where(s => !string.IsNullOrEmpty(s)).ToList());
						} catch (JsonReaderException e) {
							description = description.Replace("\\x", "\\\\x"); // replace "\x" with "\\x"
							description = string.Join("", JToken.Parse(description).ToObject<string[]>()
												.Where(s => !string.IsNullOrEmpty(s)).ToList());
						}
					} else
						description = "NODATA";
					
					
					string features = extractedProducts.description[key.Name].Value;
					if (features != null) {
						try {
							features = string.Join(" || ", JToken.Parse(features).ToObject<string[]>()
												.Where(s => !string.IsNullOrEmpty(s)).ToList());
						} catch (JsonReaderException e) {
							features = features.Replace("\\x", "\\\\x"); // replace "\x" with "\\x"
							features = string.Join(" || ", JToken.Parse(features).ToObject<string[]>()
												.Where(s => !string.IsNullOrEmpty(s)).ToList());
						}
					} else
						features = "UnKnown";
					
					string[] highResImageURLs = JToken.Parse(extractedProducts.imageURLHighRes[key.Name].Value.ToString()).ToObject<string[]>();
					
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
						Title = extractedProducts.title[key.Name].ToString().Trim(),
						// Description = string.Join(". ", descriptions.Where(s => !string.IsNullOrEmpty(s))),
						Description = description,
						Price = (decimal)(random.NextDouble() * 900 + 100),
						Discount = 0,
						
                        // Price = price,
						// Discount = discount,
						
                        Status = ProductStatus.Current,
						DateAdded = DateTime.ParseExact(dateDict[key.Value.ToString()], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), // DateTime.Now, // The oldest datetime among the 150794 products is: 971136000 -> 2000-10-10 02:00:00
						Brand = extractedProducts.brand[key.Name].ToString().Trim(),
						VoteCount = int.Parse(extractedProducts.vote_count[key.Name].ToString().Trim()),
						VoteAverage = double.Parse(extractedProducts.vote_average[key.Name].ToString().Trim()),
						// MainCategory = extractedProducts.main_cat[key.Name].ToString().Trim(),
						Categories = string.Join(" || ", categories.Where(s => !string.IsNullOrEmpty(s)).ToList()),
						Features = features,
						HighResImageURLs = string.Join(" || ", highResImageURLs.Where(s => !string.IsNullOrEmpty(s)).ToList()),
					};
					
					products.Add(product);
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
