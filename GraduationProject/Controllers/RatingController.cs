using AutoMapper;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
		private readonly ILogger<RatingController> _logger;
		private readonly IMapper _mapper;

		public RatingController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<RatingController> logger, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
            _userManager = userManager;
			_logger = logger;
			_mapper = mapper;
		}
        
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("ratings/getAllRatings")]
		public async Task<IActionResult> GetAllRatings()
		{
			try
			{
				var ratings = await _unitOfWork.Ratings.GetAllAsync();

				return Ok(_mapper.Map<IList<RatingDto>>(ratings));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong when trying to access all the rating data.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
        
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Route("ratings/user/{userId}/product/{productId}")]
		public async Task<IActionResult> GetRating(string userId, string productId)
		{
			try
			{
				var rating = await _unitOfWork.Ratings.GetByAsync(r => r.UserId == userId && r.ProductId == productId);

				return Ok(_mapper.Map<RatingDto>(rating));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the rating with userId={userId} and productId={productId}.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
        
        [HttpGet]
		[Route("ratings/seed")]
		public async Task<IActionResult> SeedDatabase()
		{
			List<RatingDto> ratings = RatingSeeder.Seed(null);
			await _unitOfWork.Ratings.InsertRangeAsync(_mapper.Map<List<Rating>>(ratings));
			
			// string output = "";
			// foreach (var rating in ratings)
			// {
			// 	output += rating.RatingValue.ToString() + ", ";
			// }

			await _unitOfWork.Save();
			return Ok("Data added successfully."); // + output );
		}
    }
}