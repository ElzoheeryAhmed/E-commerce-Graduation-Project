//using Microsoft.Build.Framework;

using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Services.SecurityServices
{
    public class UserRegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }


    }
}
