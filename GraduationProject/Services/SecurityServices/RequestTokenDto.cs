//using Microsoft.Build.Framework;


using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Services.SecurityServices
{
    public class RequestTokenDto
    { 
        ///  <value> Email address </value>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        ///  <value> Password </value>
        [Required]
        public string Password { get; set; }


    }
}
