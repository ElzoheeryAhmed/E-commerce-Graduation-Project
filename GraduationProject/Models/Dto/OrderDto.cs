using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models.Dto
{
    public class OrderDto
    {
        [Required]
        public string ShippingAddress { get; set; }
        
        [Required]
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
   
    public class OrderShippingDto
    {

        [Required]
        public int OrderId { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

    }


}
