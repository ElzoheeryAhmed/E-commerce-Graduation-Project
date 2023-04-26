using System.Drawing.Text;
using System.IdentityModel.Tokens.Jwt;
using GraduationProject.Controllers.Helpers;
using System.Security.Claims;
using System.Text;
using GraduationProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace GraduationProject.Services.SecurityServices
{
    //service separate logic of register & login process from endpoint logic
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly JWT _jwt;
        public AuthService(UserManager<User> userManager,IOptions<JWT> jwt)
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
                Gender = dto.Gender,
                Birthdate = dto.Birthdate,  
                PhoneNumber=dto.PhoneNumber
            };

            //process of adding user to the database
            var result = await _userManager.CreateAsync(user, dto.Password);

            //if some errors occurs and user registration is not successfully completed
            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }
            //coming here means user is already registered in the database

            //now we add role to the user
            await _userManager.AddToRoleAsync(user, "User");

            //we don`t settle with registration we will authenticate it
            //create token for the register user
            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
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
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //generate unique number in the world
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_jwt.DurationInHours),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
            

        }
        
    }
}
