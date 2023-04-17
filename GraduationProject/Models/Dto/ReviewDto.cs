using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationProject.Models.Dto
{
    public class ReviewCreateDto
    {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string ProductId { get; set; }
        
        [Required]
        public string ReviewText { get; set; }
    }
    
    public class ReviewDto : ReviewCreateDto
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
    public class ReviewUpdateDto
    {
        public string UserId { get; set; }
        
        public string ProductId { get; set; }
        
        [Required]
        public string ReviewText { get; set; }
    }
}
