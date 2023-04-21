
namespace GraduationProject.Models.Dto
{
    public class ProductCategoryCreateDto {
        public string Name { get; set; }
    }
    
    public class ProductCategoryDto : ProductCategoryCreateDto {
        public int Id { get; set; }
    }
    
    public class ProductCategoryGetDto {
        public int Id { get; set; }
    }
}
