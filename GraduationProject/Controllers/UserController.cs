using AutoMapper;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Services.SecurityServices;
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
			_unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
			_mapper = mapper;
			_logger = logger;
            _authService = authService;

        }

        [HttpGet]
        [Route("users/getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(_mapper.Map<IList<UserCreateDto>>(await _userManager.Users.ToListAsync()));
        }

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


        [HttpPost]
        
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDto userdto)
        {
            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var result = await _authService.RegisterAsync(userdto);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);


            return Ok(result);
            
        }

    }
}
