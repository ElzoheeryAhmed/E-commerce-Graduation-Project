using GraduationProject.Models;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Services.SecurityServices
{
    public class UpdateUserDto
    {
        [StringLength(15, MinimumLength = 3)]
        public string? FirstName { get; set; }


        [StringLength(15, MinimumLength = 3)]
        public string? LastName { get; set; }


        [StringLength(15, MinimumLength = 3)]
        public string? UserName { get; set; }

        [EnumDataType(typeof(Gender), ErrorMessage = "Enter:0 for Male, and 1 for Female")]
        public Gender? Gender { get; set; }


        //[Range(typeof(DateTime),(DateTime.Now.AddYears(-15)).ToString("yyyy-MM-dd"), DateTime.Now.AddYears(-100).ToString("yyyy-MM-dd"))]
        public DateTime? Birthdate { get; set; }

        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }



    }
}
