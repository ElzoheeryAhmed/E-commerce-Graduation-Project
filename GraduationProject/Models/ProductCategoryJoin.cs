
namespace GraduationProject.Models
{
    public class ProductCategoryJoin {
        public string ProductId { get; set; }
        public int ProductCategoryId { get; set; }
        public Product Product { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}
