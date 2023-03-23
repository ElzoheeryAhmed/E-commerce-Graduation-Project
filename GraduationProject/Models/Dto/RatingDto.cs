
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Models.Dto
{
    public class RatingCreateDto
    {
        public string UserId { get; set; }
        
        public string ProductId { get; set; }
        public double RatingValue { get; set; }
    }
    
    public class RatingDto : RatingCreateDto
    {
        public DateTime Timestamp { get; set; }
    }
}