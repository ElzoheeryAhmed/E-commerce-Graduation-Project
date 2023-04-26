namespace GraduationProject.Services.SecurityServices
{
    //used for dependacy injection 
    public interface IAuthService
    {
        //Register task
        Task<AuthModel> RegisterAsync(UserRegisterDto dto);


        //GetToken task
        Task<AuthModel> GetTokenAsync(RequestTokenDto dto);

    }
}
