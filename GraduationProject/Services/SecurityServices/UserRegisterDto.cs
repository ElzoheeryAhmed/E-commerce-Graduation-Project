//using Microsoft.Build.Framework;

using GraduationProject.Models;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Services.SecurityServices
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(15,MinimumLength=3)]
        public string FirstName { get; set; }


        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string LastName { get; set; }


        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [EnumDataType(typeof(Gender),ErrorMessage="Enter:0 for Male, and 1 for Female")]
        public Gender  Gender { get; set; }


        [Required]
        //[Range(typeof(DateTime),(DateTime.Now.AddYears(-15)).ToString("yyyy-MM-dd"), DateTime.Now.AddYears(-100).ToString("yyyy-MM-dd"))]
        public DateTime Birthdate { get; set; }


        [Required]
        public string PhoneNumber { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [EnumDataType(typeof(Role), ErrorMessage = "Enter:0 for User, and 1 for Admin")]
        public Role Role { get; set; }

        [Required]
        public string Password { get; set; }


    }
}
