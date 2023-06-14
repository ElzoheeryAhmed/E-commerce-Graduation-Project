namespace GraduationProject.Services.SecurityServices
{
    //returned object after login or register
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }  //IdentityUser allow multiple roles for specific user
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
    public class AlterModel: UpdateUserDto
    {
        public string Message { get; set; }
        public bool IsAltered{ get; set; }
    }

    public class UserInfo
    {
        public string UserName;
        public string Email;
        public string FirstName;
        public string LastName;
        public string Gender;
        public DateTime Birthdate;
        public string PhoneNumber;
    }

}
