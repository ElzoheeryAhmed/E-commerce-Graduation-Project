namespace GraduationProject.Models
{
    public class OrderItem
    {

        public int OrderId { get; set; }
        public string ProductId { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }

        public byte Quantity { get; set; }

    }
}
