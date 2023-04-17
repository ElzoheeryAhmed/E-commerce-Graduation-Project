using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationProject.Models.Dto
{
    public class ProductCategoryCreateDto
    {
        public string Name { get; set; }
    }
    
    public class ProductCategoryDto : ProductCategoryCreateDto
    {
        public int Id { get; set; }
    }
}
