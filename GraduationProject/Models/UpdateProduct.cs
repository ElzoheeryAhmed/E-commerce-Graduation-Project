namespace GraduationProject.Models
{
    public class UpdateProduct
    {
        public string CurrentProductId { get; set; }
        public string ProductUpdateId { get; set; }
        public Product CurrentProduct { get; set; }
        public Product ProductUpdate { get; set; }

    }
}
