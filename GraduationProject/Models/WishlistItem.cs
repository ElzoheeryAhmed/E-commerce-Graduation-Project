namespace GraduationProject.Models
{
    public class WishlistItem
    {
        public string Name { get; set; }

        public string CustomerId { get; set; }

        public string ProductId { get; set; }

        public User Customer { get; set; }

        public Product Product { get; set; }

        public byte Quantity { get; set; }
    }
}
