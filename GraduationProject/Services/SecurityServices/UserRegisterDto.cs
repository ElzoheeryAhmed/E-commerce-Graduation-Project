namespace GraduationProject.Services.SecurityServices
{
    public class UserRegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string Gender { get; set; }
        public DateTime Birthdate { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }


    }
}
