using System.Globalization;
using AutoMapper;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraduationProject.Utils
{
	public class SeedProductDto {
		public List<ProductDto> Products { get; set; } = new List<ProductDto>();
		public List<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
		public List<string> Brands { get; set; } = new List<string>();
	}
	
	public static class ProductSeeder {
		public static SeedProductDto Seed(string? jsonFilePath, IMapper _mapper) {
			if (jsonFilePath == null) {
				jsonFilePath = Path.Combine(Environment.CurrentDirectory,
											"Data",
											"InitialSeed",
											"ProductsMetadata.json");
			}
			
			// Create an empty list to hold all the products.
			List<ProductDto> products = new List<ProductDto>();
			
			Dictionary<string, int> productCategories = new Dictionary<string, int>();
			Dictionary<string, int> brands = new Dictionary<string, int>();
			int tempCategoryIndex = 0;
			int tempBrandIndex = 0;
			
			productCategories.Add("UnKnown", tempCategoryIndex++);
			productCategories.Add("Generic", tempCategoryIndex++);
			brands.Add("UnKnown", tempBrandIndex++);
			brands.Add("Generic", tempBrandIndex++);
			
			// Parsing the json file and creating the appropriate objects.
			using (StreamReader r = new StreamReader(jsonFilePath)) {
				// Parse the JSON into a dynamic object.
				dynamic extractedProducts = JsonConvert.DeserializeObject(r.ReadToEnd());
				
				foreach (var key in extractedProducts.asin) {
					// // Parsing the price. If the price is in this format ("2.98 - $3.98"), then we have a price and a discount.
					// // The discount is the difference between the two numbers.
					// string[] parsedPrice = extractedProducts.price[key.Name].ToString().Split('-');
					// decimal price = decimal.Parse(parsedPrice[0].Trim());
					// decimal discount = parsedPrice.Length > 1 ? 0 : decimal.Parse(parsedPrice[0].Trim()) - price;
					Random random = new Random();
					
					
					// Next fields requires extra deserialization.
					string[] categories = JToken.Parse(extractedProducts.category[key.Name].Value.ToString()).ToObject<string[]>();
					
					// Filtering duplicate categories.
					HashSet<int> tempCategoryIds = new HashSet<int>();
					for (int i=0; i < categories.Length; i++) {
						string category = categories[i];
						if (!string.IsNullOrWhiteSpace(category)) {
							if (productCategories.ContainsKey(category)) {
								tempCategoryIds.Add(productCategories[category]);
							}
							
							else {
								productCategories.Add(category, tempCategoryIndex);
								tempCategoryIds.Add(tempCategoryIndex++);
							}
						}
					}
					
					List<ProductCategory> categoryIds = new List<ProductCategory>();
					if (tempCategoryIds.Count == 0) {
						categoryIds.Add(new ProductCategory() { Id = 1 });
					}
					
					else {
						categoryIds.AddRange(tempCategoryIds.Select(id => new ProductCategory() { Id = id }));
					}
					
					// Parsing if the description does not contain any nulls: string[] descriptions = JToken.Parse(extractedProducts.description[key.Name].Value.ToString()).ToObject<string[]>();
					// Description can take null values in the dataset, thus we need to be extra careful when parsing it.
					string description = extractedProducts.description[key.Name].Value;
					if (description != null) {
						try {
							description = string.Join("", JToken.Parse(description).ToObject<string[]>()
												.Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
						} catch (JsonReaderException) {
							description = description.Replace("\\x", "\\\\x"); // replace "\x" with "\\x"
							description = string.Join("", JToken.Parse(description).ToObject<string[]>()
												.Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
						}
					}
					
					else
						description = "NODATA";
					
					
					string features = extractedProducts.description[key.Name].Value;
					if (features != null) {
						try {
							features = string.Join(" || ", JToken.Parse(features).ToObject<string[]>()
												.Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
						}
						
						catch (JsonReaderException) {
							features = features.Replace("\\x", "\\\\x"); // replace "\x" with "\\x"
							features = string.Join(" || ", JToken.Parse(features).ToObject<string[]>()
												.Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
						}
					}
					
					else
						features = "UnKnown";
					
					string[] highResImageURLs = JToken.Parse(extractedProducts.imageURLHighRes[key.Name].Value.ToString()).ToObject<string[]>();
					
					// Parsing addedDates.
					jsonFilePath = Path.Combine(Environment.CurrentDirectory,
											"Data",
											"InitialSeed",
											"product_added_dates.json");
					
					var dateDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonFilePath));
					
					string tempBrand = extractedProducts.brand[key.Name].ToString().Trim();
					int brandId = 1; // 0 = Generic product (doesn't have a brand).
					if (!string.IsNullOrWhiteSpace(tempBrand)) {
						if (brands.ContainsKey(tempBrand)) {
							brandId = brands[tempBrand];
						}
						
						else {
							brands.Add(tempBrand, tempBrandIndex);
							brandId = tempBrandIndex++;
						}
					}
					
					// Creating a Product object.
					ProductDto product = new ProductDto {
						Id = key.Value.ToString(),
						Title = extractedProducts.title[key.Name].ToString().Trim(),
						// Description = string.Join(". ", descriptions.Where(s => !string.IsNullOrWhiteSpace(s))),
						Description = description,
						Price = (decimal)(random.NextDouble() * 900 + 100), // 100 <= price <= 1000
						Discount = 0,
						
                        // Price = price,
						// Discount = discount,
						
                        // Status = ProductStatus.Current,
						DateAdded = DateTime.ParseExact(dateDict[key.Value.ToString()], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), // DateTime.Now, // The oldest datetime among the 150794 products is: 971136000 -> 2000-10-10 02:00:00
						BrandId = brandId,
						VoteCount = int.Parse(extractedProducts.vote_count[key.Name].ToString().Trim()),
						VoteAverage = double.Parse(extractedProducts.vote_average[key.Name].ToString().Trim()),
						// MainCategory = extractedProducts.main_cat[key.Name].ToString().Trim(),
						Quantity = (int)(random.NextDouble() * 5 + 5), // 5 <= quantity <= 10
						ProductCategories = _mapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(categoryIds), // string.Join(" || ", categories.Where(s => !string.IsNullOrWhiteSpace(s)).ToList())
						Features = features,
						HighResImageURLs = string.Join(" || ", highResImageURLs.Where(s => !string.IsNullOrWhiteSpace(s)).ToList())
					};
					
					products.Add(product);
				}
				
				List<ProductCategory> productCategoriesList = new List<ProductCategory>();
				
				foreach (var productCategory in productCategories.OrderBy(pc => pc.Value)) {
					productCategoriesList.Add(new ProductCategory {Name = productCategory.Key});
				}
				
				List<string> brandNames = new List<string>();
				foreach (var brandName in brands.OrderBy(pc => pc.Value)) {
					brandNames.Add(brandName.Key);
				}
				
				SeedProductDto seedProductDto = new SeedProductDto {
					Products = products,
					Brands = brandNames,
					ProductCategories = productCategoriesList,
				};
				
				return seedProductDto;
			}
		}
	}
}


// To revert back to an older migration:
	// Update-Database -Migration:InitialCreate
	// Remove-Migration
	// Add-Migration Product-InitialCreate
	// Update-Database
