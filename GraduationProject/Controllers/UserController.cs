using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Services.SecurityServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        private readonly IAuthService _authService;
        public UserController(IUnitOfWork unitOfWork, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, ILogger<UserController> logger, IAuthService authService)
        {
           // _unitOfWork = unitOfWork;
            //_signInManager = signInManager;
           // _mapper = mapper;
           // _logger = logger;
            _authService = authService;

        }

        // [HttpGet]
        //[Route("users/getAllUsers")]
        /*[HttpGet(template: "getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(_mapper.Map<IList<UserCreateDto>>(await _userManager.Users.ToListAsync()));
        }
        */
        /*
        
		[HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserCreateDto userDto)
        {
            _logger.LogInformation($"Registering a user with email {userDto.Email}.");
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var user = _mapper.Map<User>(userDto);
                var result = await _userManager.CreateAsync(user, user.Email);
                
                if (!result.Succeeded)
                    return BadRequest("Something went wrong while registering a new user with email: " + userDto.Email);
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a new user with email: " + userDto.Email);
                return Problem($"An error occurred while registering a new user with email: " + userDto.Email, statusCode: 500);
            }
        }*/

        [HttpGet(template: "Login")]
        public async Task<IActionResult> LoginAsync([FromQuery] RequestTokenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(dto);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message); //return only the error message

            return Ok(new { Username = result.Username, ExpireDate = result.ExpiresOn, Roles = result.Roles, token = result.Token }); //We can  exclude the message from the result, it will be always empty 
        }
        [Authorize(Roles="User , Admin")]
        [HttpGet(template: "GetPersonalInfo")]
        public async Task<IActionResult> GetInfoAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userInfo = await _authService.GetInfoAsync(userId);
            return Ok(userInfo);
        }

        [HttpPost(template: "Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDto userdto)
        {

            //Check for violation of the constraints that escaped from binding
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var result = await _authService.RegisterAsync(userdto);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);


            return Ok(new { Username = result.Username, ExpireDate = result.ExpiresOn, Roles = result.Roles, token = result.Token }); //We can  exclude the message from the result, it will be always empty

        }

        [Authorize(Roles = "User , Admin")]
        [HttpPut(template: "UpdateInfo")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserDto dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var result = await _authService.UpdateAsync(dto, userId);

            if (!result.IsAltered)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { UserName=result.UserName, Email=result.Email, FirstName=result.FirstName, LastName=result.LastName,Gender=result.Gender.ToString(), PhoneNumber = result.PhoneNumber, Birthdate = result.Birthdate});
        }

        [Authorize(Roles = "User , Admin")]
        [HttpPut(template: "ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromQuery][Required] string currentPassword, [FromQuery][Required] string newPassword)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var alterModel = await _authService.ChangePasswordAsync(userId, currentPassword, newPassword);

            if (alterModel.IsAltered)
            {
                return Ok("Password is changed successfully");
            }
            else
            {
                return BadRequest(alterModel.Message);
            }

        }
    }
}
