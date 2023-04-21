
namespace GraduationProject.Models
{
    public class ProductUpdate {
        public string CurrentProductId { get; set; }
        public string UpdatedProductId { get; set; }
        public Product CurrentProduct { get; set; }
        public Product UpdatedProduct { get; set; }

    }
}
