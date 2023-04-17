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
		
		[Required]
		public string Title { get; set; }
		
		[Required]
		public string? Description { get; set; }
		
		[Required]
    	[Range(0.01, 100_000_0000.0, ErrorMessage = "Price must be greater than 0. Max price allowed is $1 Million.")]
		public decimal Price { get; set; }
		
		[Range(0.0, 100_000_0000.0, ErrorMessage = "Discount must be greater than or equal to 0. Max discount allowed is $1 Million.")]
		public decimal Discount { get; set; }
		
		public ProductStatus Status { get; set; }
		
		public DateTime DateAdded { get; set; }

		public int VoteCount { get; set; }
		public double VoteAverage { get; set; }
		// public string MainCategory { get; set; }
		public string Features { get; set; }
		public string HighResImageURLs { get; set; }
		
		public int BrandId { get; set; }
		public Brand Brand { get; set; }
		public List<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
		public ICollection<Rating> Ratings { get; set; } = new HashSet <Rating>();
		public ICollection<Review> Reviews { get; set; } = new HashSet <Review>();
	}
}
