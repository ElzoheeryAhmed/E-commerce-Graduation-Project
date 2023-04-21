using AutoMapper;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
		private readonly ILogger<RatingController> _logger;
		private readonly IMapper _mapper;

		public RatingController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<RatingController> logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
            _userManager = userManager;
			_logger = logger;
			_mapper = mapper;
		}
        
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GerRatings([FromQuery] PagingFilter pagingFilter) {
			try {
				var ratings = await _unitOfWork.Ratings.GetAllAsync(pagingFilter: pagingFilter);

				return Ok(_mapper.Map<IList<RatingDto>>(ratings));
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access all the rating data.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
        
        [HttpGet("{userId}/{productId}")]
		[ActionName(nameof(GetRating))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRating(string userId, string productId) {
			try {
				var rating = await _unitOfWork.Ratings.GetByAsync(r => r.UserId == userId && r.ProductId == productId);
				
				if (rating == null) {
					_logger.LogWarning($"Failed GET attempt in {nameof(GetRating)}. Could not find a rating with userId={userId} and productId={productId}.");
					
					return NotFound($"Could not find a rating with userId={userId} and productId={productId}");
				}
				
				return Ok(_mapper.Map<RatingDto>(rating));
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the rating with userId={userId} and productId={productId}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateRating([FromBody] RatingCreateDto ratingDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Failed POST attempt in {nameof(CreateRating)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			try {
				var rating = _mapper.Map<Rating>(ratingDto);
				
				var exists = await _unitOfWork.Ratings.existsAsync(r => r.ProductId == rating.ProductId && r.UserId == rating.UserId);
				
				if (exists) {
					_logger.LogInformation($"Invalid POST attempt in {nameof(CreateRating)}. The current user has already rated the same product before.");
					
					return BadRequest("The current user has already rated the same product before.");
				}
				
				await _unitOfWork.Ratings.InsertAsync(rating);
				await _unitOfWork.Save();
				
				return CreatedAtRoute("GetRating", new { Id = rating.ProductId }, rating);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to create a new rating in the {nameof(CreateRating)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		[HttpPut("{userId}/{productId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateRating(string userId, string productId, [FromBody] RatingUpdateDto ratingDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateRating)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			if (userId != ratingDto.UserId || productId != ratingDto.ProductId) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateRating)}. The user id and product id in the request body do not match the ones in the request url.");
				
				return BadRequest("The user id and product id in the request body do not match the ones in the request url.");
			}
			
			try {
				var rating = _mapper.Map<Rating>(ratingDto);
				
				var exists = await _unitOfWork.Ratings.existsAsync(r => r.ProductId == productId && r.UserId == userId);
				
				if (!exists) {
					_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateRating)}. The user with id={userId} has not rated the product with product id={productId} before.");
					
					return NotFound($"The user with id={userId} has not rated the product with product id={productId} before.");
				}
				
				rating.Timestamp = DateTime.Now;
				
				_unitOfWork.Ratings.Update(rating);
				
				await _unitOfWork.Save();
				
				return NoContent();
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to update a rating in the {nameof(UpdateRating)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		[HttpDelete("{userId}/{productId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteRating(string userId, string productId) {
			if (userId == null || productId == null) {
				_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteRating)}. Either the user id or product id is null.");
				
				return BadRequest(ModelState);
			}
			
			try {
				var exists = await _unitOfWork.Ratings.existsAsync(r => r.UserId == userId && r.ProductId == productId);
				
				if (!exists) {
					_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteRating)}. The user with id={userId} has not rated the product with product id={productId} yet.");
					
					return NotFound($"The user with id={userId} has not rated the product with product id={productId} yet.");
				}
				
				await _unitOfWork.Ratings.DeleteAsync(r => r.UserId == userId && r.ProductId == productId);
				await _unitOfWork.Save();
				
				return NoContent();
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to delete a rating in the {nameof(DeleteRating)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
    }
}
