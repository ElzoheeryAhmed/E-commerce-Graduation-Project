
namespace GraduationProject.Models
{
    /// <summary>
    /// This class defines a many-to-many relationship between the User and Product entities. The Rating entity acts as the join table that represents the ratings given by users to products.
    /// </summary>
    public class Rating
    {
        public string UserId { get; set; }
        public User User { get; set; }
        
        public string ProductId { get; set; }
        public Product Product { get; set; }
        
        public double RatingValue { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
