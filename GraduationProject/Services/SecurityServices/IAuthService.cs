using GraduationProject.Models;

namespace GraduationProject.Services.SecurityServices
{
    //used for dependacy injection 
    public interface IAuthService
    {
        //Register task
        Task<AuthModel> RegisterAsync(UserRegisterDto dto);


        //GetToken task
        Task<AuthModel> GetTokenAsync(RequestTokenDto dto);
        Task<UserInfo> GetInfoAsync(string userId);

        Task<AlterModel> UpdateAsync(UpdateUserDto dto, string userId);
        Task<AlterModel> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        

    }
}
