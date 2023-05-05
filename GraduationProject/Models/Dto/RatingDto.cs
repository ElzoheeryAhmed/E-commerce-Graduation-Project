
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models.Dto
{
    public class RatingCreateDto {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ProductId { get; set; }
        
        [Required]
        [Range(0, 5, ErrorMessage = "Rating value must be between 0 and 5.")]
        public double RatingValue { get; set; }
    }
    
    public class RatingDto : RatingCreateDto {
        public DateTime Timestamp { get; set; }
    }
    
    public class RatingUpdateDto {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        
        [Required]
        [Range(0, 5, ErrorMessage = "Rating value must be between 0 and 5.")]
        public double RatingValue { get; set; }
    }
}
