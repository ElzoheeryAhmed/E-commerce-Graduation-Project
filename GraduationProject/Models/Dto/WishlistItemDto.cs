using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models.Dto
{
    
    public class WishlistItemDetailsDto
    {

        public string ProductId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string HighResImageURLs { get; set; }

        public byte Quantity { get; set; }
    }


    

    public class WishlistIdentifyDto
    {
        public string Name { get; set; }
        public string CustomerId { get; set; }



    }
    public class WishlistItemIdentifyDto: WishlistIdentifyDto
    {
        
        public string ProductId { get; set; }

    }

    public class WishlistItemDto: WishlistItemIdentifyDto
    {
        [Range(1,255)]
        public byte Quantity { get; set; }
    }

}
