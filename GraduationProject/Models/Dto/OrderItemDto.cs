using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models.Dto
{
    public class OrderItemDto
    {

        public string ProductId { get; set; }
        
        [Range(1,255)]
        public byte Quantity { get; set; }


    }

    public class OrderItemDetailsDto
    {

        
        public string ProductId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string HighResImageURLs { get; set; }

        public byte Quantity { get; set; }

    }


    }
