namespace GraduationProject.Models.Dto
{
    public class WishlistItemDto
    {
        public string Name { get; set; }    
        public string CustomerId { get; set; }

        public string ProductId { get; set; }

        public byte Quantity { get; set; }
    }


    public class WishlistItemDetailsDto
    {

        public string ProductId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string HighResImageURLs { get; set; }

        public byte Quantity { get; set; }
    }


    public class WishlistItemIdentifyDto
    {
        public string Name { get; set; }
        public string CustomerId { get; set; }

        public string ProductId { get; set; }

    }

}
