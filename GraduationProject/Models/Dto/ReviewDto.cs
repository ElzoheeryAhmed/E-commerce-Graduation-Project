using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationProject.Models.Dto
{
    public class ReviewCreateDto
    {
        public string UserId { get; set; }
        
        public string ProductId { get; set; }
        
        public string ReviewText { get; set; }
    }
    
    public class ReviewDto : ReviewCreateDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
