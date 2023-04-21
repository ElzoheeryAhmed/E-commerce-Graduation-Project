
namespace GraduationProject.Models
{
    public class ProductCategory {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<ProductCategoryJoin> Products { get; set; } = new HashSet<ProductCategoryJoin>();
    }
}
