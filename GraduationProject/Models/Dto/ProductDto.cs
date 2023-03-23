using GraduationProject.Models.ModelEnums;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models.Dto
{
	public class ProductCreateDto
	{
		[Required]
		public string Title { get; set; }                   // 3
		
		[Required]
		public string Description { get; set; }             // 2
		
		[Required]
		[Range(0.01, 100_000_0000.0, ErrorMessage = "Price must be greater than 0. Max price allowed is $1 Million.")]
		public decimal Price { get; set; }                  // 9
		
		[Range(0.0, 100_000_0000.0, ErrorMessage = "Discount must be greater than or equal to 0. Max discount allowed is $1 Million.")]
		public decimal Discount { get; set; }               // ---
		
		public string? Brand { get; set; }                  // 5
		
		// public string MainCategory { get; set; }         // 8
		public string Categories { get; set; }              // 1  (Separated by ' || ')
		public string Features { get; set; }                // 6  (Separated by ' || ')
		public string HighResImageURLs { get; set; }        // 11 (Separated by ' || ')
	}
	
	public class ProductDto : ProductCreateDto
	{
		public string Id { get; set; }
		public ProductStatus Status { get; set; }           // ---
		public int VoteCount { get; set; }                  // 12
		public DateTime DateAdded { get; set; }
		public double VoteAverage { get; set; }             // 13
	}
}
