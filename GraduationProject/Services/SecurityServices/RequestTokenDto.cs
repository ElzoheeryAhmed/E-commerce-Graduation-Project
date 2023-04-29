﻿//using Microsoft.Build.Framework;


using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Services.SecurityServices
{
    public class RequestTokenDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }


    }
}