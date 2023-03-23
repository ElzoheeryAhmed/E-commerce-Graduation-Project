using GraduationProject.Models.ModelEnums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Models
{
	public class Product
	{
		[Key]
    	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Id { get; set; } // Guid.NewGuid().ToString();
		// public string ProductId { get; set; }			// 10
		
		[Required]
		public string Title { get; set; }					// 3
		
		[Required]
		public string Description { get; set; }				// 2
		
		[Required]
    	[Range(0.01, 100_000_0000.0, ErrorMessage = "Price must be greater than 0. Max price allowed is $1 Million.")]
		public decimal Price { get; set; }					// 9
		
		[Range(0.0, 100_000_0000.0, ErrorMessage = "Discount must be greater than or equal to 0. Max discount allowed is $1 Million.")]
		public decimal Discount { get; set; }               // ---
		
		public ProductStatus Status { get; set; }			// ---
		
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime DateAdded { get; set; }				// ---

		public string? Brand { get; set; }					// 5
		public int VoteCount { get; set; }					// 12
		public double VoteAverage { get; set; }				// 13
		// public string MainCategory { get; set; }			// 8
		public string Categories { get; set; }              // 1  (Separated by ' || ')
		public string Features { get; set; }				// 6  (Separated by ' || ')
		public string HighResImageURLs { get; set; }		// 11 (Separated by ' || ')
		
		public ICollection<Rating> Ratings { get; set; }
		public ICollection<Review> Reviews { get; set; }
		
		//public List<int>? AlsoBoughtProductIds { get; set; }// 4
		//public List<int>? AlsoViewedProductIds { get; set; }// 7
		
		// TODO: Add a new field `Overall_Rating`.
		// TODO: Add a new field `TimesAccessed`. When this time reaches, like, 100 times, a new access triggers the update of the overall_rating of the product.
		
		public Product()
		{
			this.Ratings = new List<Rating>();
			this.Reviews = new List<Review>();
		}
	}
}


// category
// description
// title
// also_buy
// brand
// feature
// also_view
// main_cat
// price
// asin
// imageURLHighRes
// vote_count
// vote_average