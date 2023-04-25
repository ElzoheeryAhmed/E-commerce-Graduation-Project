namespace GraduationProject.Models.Dto
{

    public class CartItemDto
    {

        public string ProductId { get; set; }

        public string CustomerId { get; set; }
        public byte Quantity { get; set; }


    }

    public class CartItemkeyDto
    {

        public string ProductId { get; set; }

        public string CustomerId { get; set; }


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
