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
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
		private readonly ILogger<ReviewController> _logger;
		private readonly IMapper _mapper;

		public ReviewController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<ReviewController> logger, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
            _userManager = userManager;
			_logger = logger;
			_mapper = mapper;
		}
        
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("reviews/getAllReviews")]
		public async Task<IActionResult> GetAllReviews()
		{
			try
			{
				var reviews = await _unitOfWork.Reviews.GetAllAsync();

				return Ok(_mapper.Map<IList<ReviewDto>>(reviews));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong when trying to access all the review data.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
        
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Route("reviews/user/{userId}/product/{productId}")]
		public async Task<IActionResult> GetReview(string userId, string productId)
		{
			try
			{
				var review = await _unitOfWork.Reviews.GetByAsync(r => r.UserId == userId && r.ProductId == productId);

				return Ok(_mapper.Map<ReviewDto>(review));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the review with userId={userId} and productId={productId}.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
        
        [HttpGet]
		[Route("reviews/seedClothings")]
		public async Task<IActionResult> SeedClothingDatabase()
		{
			List<ReviewDto> reviews = ReviewSeeder.Seed(0);
			await _unitOfWork.Reviews.InsertRangeAsync(_mapper.Map<List<Review>>(reviews));
			string output = "";
			foreach (var review in reviews)
			{
				output += review.ReviewText.ToString() + ", ";
			}

			await _unitOfWork.Save();
			return Ok("Data added successfully: " + output );
		}
        
        [HttpGet]
		[Route("reviews/seedHome_Kitchen")]
		public async Task<IActionResult> SeedHome_KitchenDatabase()
		{
			List<ReviewDto> reviews = ReviewSeeder.Seed(1);
			await _unitOfWork.Reviews.InsertRangeAsync(_mapper.Map<List<Review>>(reviews));
			
            // string output = "";
			// foreach (var review in reviews)
			// {
			// 	output += review.ReviewText.ToString() + ", ";
			// }

			await _unitOfWork.Save();
			return Ok("Data added successfully."); // " + output );
		}
    }
}