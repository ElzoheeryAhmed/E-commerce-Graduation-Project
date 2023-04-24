using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models.Dto
{
    public class ReviewCreateDto {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string ProductId { get; set; }
        
        [Required]
        public string ReviewText { get; set; }
    }
    
    public class ReviewDto : ReviewCreateDto {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
    public class ReviewUpdateDto {
        [Required]
        public string ReviewText { get; set; }
    }
}
