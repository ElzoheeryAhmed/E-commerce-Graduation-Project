using System.Drawing.Text;
using System.IdentityModel.Tokens.Jwt;
using GraduationProject.Controllers.Helpers;
using System.Security.Claims;
using System.Text;
using GraduationProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace GraduationProject.Services.SecurityServices
{
    //service separate logic of register & login process from endpoint logic
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly JWT _jwt;
        public AuthService(UserManager<User> userManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
        }

        public async Task<AuthModel> RegisterAsync(UserRegisterDto dto)
        {
            //Check that  email is not exist before
            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            //Check that user name is not exists before
            if (await _userManager.FindByNameAsync(dto.UserName) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender.ToString(),
                Birthdate = dto.Birthdate,
                PhoneNumber = dto.PhoneNumber
            };

            //process of adding user to the database
            var result = await _userManager.CreateAsync(user, dto.Password);

            //if some errors occurs and user registration is not successfully completed
            if (!result.Succeeded)
            {
                return new AuthModel { Message = SecurityHelper.parseError(result.Errors) };
            }
            //coming here means user is already registered in the database


            //now we add role to the user
            if(dto.Role==Role.User)
                await _userManager.AddToRoleAsync(user, "User");
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            //we don`t settle with registration we will authenticate it
            //create token for the register user
            var jwtSecurityToken = await CreateJwtToken(user);
            //get user roles
            var rolesList = await _userManager.GetRolesAsync(user);
            return new AuthModel
            {   
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = rolesList.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
        }

        //GetToken method
        public async Task<AuthModel> GetTokenAsync(RequestTokenDto dto)
        {
            var authModel = new AuthModel();

            //check if there is a user with this email or not 
            //if user is null, then Email is incorrect---unless email is exist with user 
            //after email is exist, we will check that the user with this email have the
            //same password as the sent in the request          
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                //Annonomous message
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            //get jwtsecuritytoken object 
            var jwtSecurityToken = await CreateJwtToken(user);
            //get user roles I didn`t know if we need it  here or not 
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken); //retrive token as string 
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;

        }

        //private method used only be register and login methods
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            //set claims which is used in token generation process  
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //generate unique number in the world
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userName", user.UserName)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_jwt.DurationInHours), //return datetime object with value: currentTime+ expire period
                signingCredentials: signingCredentials);

            return jwtSecurityToken;


        }
        public async Task<UserInfo> GetInfoAsync(string userId)
        {
            
            var user = await _userManager.FindByIdAsync(userId);

            return new UserInfo()   {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender.ToString(),
                Birthdate = user.Birthdate,
                PhoneNumber = user.PhoneNumber
            };
        }
        public async Task<AlterModel> UpdateAsync(UpdateUserDto dto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);


            if ((dto.UserName is not null)&&(dto.UserName != user.UserName))
            {
                //Check that  username is not exist before
                if (await _userManager.FindByNameAsync(dto.UserName) is not null)
                    return new AlterModel { Message = "Username is already existed!" };
            }

            if ((dto.Email is not null) && (dto.Email != user.Email))
            {
                //Check that  email is not exist before
                if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                    return new AlterModel { Message = "Email is already existed!" };
            }

            //Updating User information
            user.UserName = dto.UserName ?? user.UserName;
            user.Email = dto.Email ?? user.Email;
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName?? user.LastName;
            user.Gender = dto?.Gender?.ToString()?? user.Gender;
            user.Birthdate = dto.Birthdate??user.Birthdate;
            user.PhoneNumber = dto.PhoneNumber??user.PhoneNumber;


            //process of updating user in the database
            var result = await _userManager.UpdateAsync(user);

            //if some errors occurs and user updating is not successfully completed
            if (!result.Succeeded)
            {
                return new AlterModel { Message = SecurityHelper.parseError(result.Errors), IsAltered = false };
            }
            else {
                 user = await _userManager.FindByIdAsync(user.Id);

                return new AlterModel
                {   IsAltered = true,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = (user.Gender=="Male")?Gender.Male:Gender.Female,
                    Birthdate = user.Birthdate,
                    PhoneNumber = user.PhoneNumber,
            };
            }
        }


        public async Task<AlterModel> ChangePasswordAsync(string userId,string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                return new AlterModel { Message = SecurityHelper.parseError(result.Errors), IsAltered = false };

            }
            else
            {
                return new AlterModel  { IsAltered = true };
            }
        }





    }
}
