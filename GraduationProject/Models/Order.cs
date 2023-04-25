namespace GraduationProject.Models
{

        public enum OrderStatus : byte
        {
            Confirmed,Shipped, Cancelled, Receipted,Returned
        }
        public class Order
        {
        
            public int Id { get; set; }
            public OrderStatus Status { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime? ReceiptDate { get; set; }
            public string ShippingAddress { get; set; }
            public string CustomerId { get; set; }
            public ICollection<OrderItem> OrderItems { get; set; }
            public User Customer { get; set; }
        }
 }

