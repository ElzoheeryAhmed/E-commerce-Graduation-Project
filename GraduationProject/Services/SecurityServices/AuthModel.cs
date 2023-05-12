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
    public class UpdateModel
    {
        public string Message { get; set; }
        public bool IsUpdated { get; set; }
    }



}
