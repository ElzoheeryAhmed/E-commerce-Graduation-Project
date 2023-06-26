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
            _logger = logger;
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
        /// <summary>
        /// Validate user credentials
        /// </summary>
        /// <param name="dto">user credentials</param>
        /// <returns></returns>
        /// <response code="200">Returns username, roles, expiredate and access token</response>
		/// <response code="400">Badrequest, Credentials are invalid</response>
        /// <response code="500">Unexpected internal server error occured</response>

        [HttpGet(template: "Login")]
        public async Task<IActionResult> LoginAsync([FromQuery] RequestTokenDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.GetTokenAsync(dto);

                if (!result.IsAuthenticated)
                    return BadRequest(result.Message); //return only the error message

                return Ok(new { Username = result.Username, ExpireDate = result.ExpiresOn, Roles = result.Roles, token = result.Token }); //We can  exclude the message from the result, it will be always empty 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during verifing user credentials with email{dto.Email}.");
                return StatusCode(500, "Internal Server Error.");
            }
        }

        /// <summary>
        /// Retrieve user personal information
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns user personal information</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [Authorize(Roles="User , Admin")]
        [HttpGet(template: "GetPersonalInfo")]
        public async Task<IActionResult> GetInfoAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var userInfo = await _authService.GetInfoAsync(userId);
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during retrieving user personal data with UserId:{userId}.");
                return StatusCode(500, "Internal Server Error.");
            }
        }


        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="userdto">Registration information, Notice gender by default male:0</param>
        /// <returns></returns>
        /// <response code="200">Returns username, roles, expiredate and access token, user is registered successfully</response>
        /// <response code="400">Badrequest, Input information for registration violate the constraints</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpPost(template: "Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDto userdto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during registering new user with username:{userdto.UserName}.");
                return StatusCode(500, "Internal Server Error.");
            }

        }


        /// <summary>
        /// Update user personal information
        /// </summary>
        /// <param name="dto">Updated data</param>
        /// <returns></returns>
        /// <response code="200">Returns user personal information after the update</response>
        /// <response code="400">Badrequest,Input information for updating violate constraints</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [Authorize(Roles = "User , Admin")]
        [HttpPut(template: "UpdateInfo")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var result = await _authService.UpdateAsync(dto, userId);

                if (!result.IsAltered)
                {
                    return BadRequest(result.Message);
                }

                return Ok(new { UserName = result.UserName, Email = result.Email, FirstName = result.FirstName, LastName = result.LastName, Gender = result.Gender.ToString(), PhoneNumber = result.PhoneNumber, Birthdate = result.Birthdate });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during updating  user personal information with userId:{userId}.");
                return StatusCode(500, "Internal Server Error.");
            }
        }
        /// <summary>
        /// Change user account password
        /// </summary>
        /// <param name="currentPassword">The current password</param>
        /// <param name="newPassword">string to be the new password</param>
        /// <returns></returns>
        /// <response code="200">Password is changed successfully</response>
        /// <response code="400">Badrequest, due to input current password is invalid or new password violate constraints </response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [Authorize(Roles = "User , Admin")]
        [HttpPut(template: "ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromQuery][Required] string currentPassword, [FromQuery][Required] string newPassword)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {

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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during changing  user password with userId:{userId}.");
                return StatusCode(500, "Internal Server Error.");
            }

        }

    }
}
