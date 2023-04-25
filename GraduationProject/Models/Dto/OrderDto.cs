namespace GraduationProject.Models.Dto
{
    public class OrderDto
    {
        
        public string CustomerId { get; set; }
        public string ShippingAddress { get; set; }

        public ICollection<OrderItemDto> OrderItems { get; set; }
        
    }


    public class OrderDetailsDto
    {

        public int Id { get; set; }
        public String Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string ShippingAddress { get; set; }

        public ICollection<OrderItemDetailsDto> OrderItems { get; set; }
        

    }
    public class OrderAdminDto:OrderDetailsDto
    {

        public string CustomerId { get; set; }


    }

}
