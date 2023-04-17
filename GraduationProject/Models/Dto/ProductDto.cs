using GraduationProject.Models.ModelEnums;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models.Dto
{
	public class ProductCreateDto
	{
		[Required]
		public string Title { get; set; }
		
		[Required]
		public string? Description { get; set; }
		
		[Required]
		[Range(0.01, 100_000_0000.0, ErrorMessage = "Price must be greater than 0. Max price allowed is $1 Million.")]
		public decimal Price { get; set; }
		
		[Range(0.0, 100_000_0000.0, ErrorMessage = "Discount must be greater than or equal to 0. Max discount allowed is $1 Million.")]
		public decimal Discount { get; set; }
		
		public int BrandId { get; set; }
		
		// public string MainCategory { get; set; }
		public List<ProductCategoryCreateDto> ProductCategories { get; set; } = new List<ProductCategoryCreateDto>();
		public string Features { get; set; }
		public string HighResImageURLs { get; set; }
	}
	
	public class ProductDto : ProductCreateDto
	{
		
		[Required]
		public string Id { get; set; }
		public ProductStatus Status { get; set; }
		public int VoteCount { get; set; }
		public DateTime DateAdded { get; set; }
		public double VoteAverage { get; set; }
		public List<ProductCategoryDto> ProductCategories { get; set; } = new List<ProductCategoryDto>();
	}
	
	
	public class ProductUpdateDto
	{
		public string? Title { get; set; }
		public string? Description { get; set; }
		
		[Range(0.01, 100_000_0000.0, ErrorMessage = "Price must be greater than 0. Max price allowed is $1 Million.")]
		public decimal Price { get; set; }
		
		[Range(0.0, 100_000_0000.0, ErrorMessage = "Discount must be greater than or equal to 0. Max discount allowed is $1 Million.")]
		public decimal Discount { get; set; }
		
		public Brand Brand { get; set; }
		
		// public string MainCategory { get; set; }
		public ICollection<ProductCategoryDto>? ProductCategories { get; set; } = new HashSet<ProductCategoryDto>();
		public string? Features { get; set; }
		public string? HighResImageURLs { get; set; }
		public ProductStatus Status { get; set; }
		public int VoteCount { get; set; }
		public double VoteAverage { get; set; }
	}
}
