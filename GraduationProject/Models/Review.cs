
namespace GraduationProject.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        
        public string ProductId { get; set; }
        public Product Product { get; set; }
        
        public string ReviewText { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}
