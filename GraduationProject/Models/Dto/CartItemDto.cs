using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models.Dto
{

    

    public class CartItemIdentifyDto
    {
        [Required]
        public string ProductId { get; set; }


    }
    public class CartItemDto: CartItemIdentifyDto
    {
        [Required,Range(1,255)]
        public byte Quantity { get; set; }


    }
    public class CartItemDetailsDto
    {
       
            public string ProductId { get; set; }

            public string Title { get; set; }

            public decimal Price { get; set; }

            public string HighResImageURLs { get; set; }

            public byte Quantity { get; set; }

        
    }
}
