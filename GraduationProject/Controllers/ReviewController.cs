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
    public class ReviewController : ControllerBase {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
		private readonly ILogger<ReviewController> _logger;
		private readonly IMapper _mapper;

		public ReviewController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<ReviewController> logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
            _userManager = userManager;
			_logger = logger;
			_mapper = mapper;
		}
        
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetReviews([FromQuery] PagingFilter pagingFilter) {
			try {
				var reviews = await _unitOfWork.Reviews.GetAllAsync(pagingFilter: pagingFilter);

				return Ok(_mapper.Map<IList<ReviewDto>>(reviews));
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access all the review data.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		
        [HttpGet("{userId}/{productId}")]
		[ActionName(nameof(GetReview))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetReview(string userId, string productId) {
			try {
				var review = await _unitOfWork.Reviews.GetByAsync(r => r.UserId == userId && r.ProductId == productId);
				
				if (review == null) {
					_logger.LogWarning($"Failed GET attempt in {nameof(GetReview)}. Could not find a review with userId={userId} and productId={productId}.");
					
					return NotFound($"Could not find a review with userId={userId} and productId={productId}");
				}
				
				return Ok(_mapper.Map<ReviewDto>(review));
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the review with userId={userId} and productId={productId}.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateReview)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			try {
				var review = _mapper.Map<Review>(reviewDto);
				
				var exists = await _unitOfWork.Reviews.existsAsync(r => r.Id == review.Id);
				
				if (exists) {
					_logger.LogInformation($"Invalid POST attempt in {nameof(CreateReview)}. A review with same key already exists.");
					
					return BadRequest("A review with same key already exists.");
				}
				
				await _unitOfWork.Reviews.InsertAsync(review);
				await _unitOfWork.Save();
				
				return CreatedAtRoute("GetReview", new { Id = review.Id }, review);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to create a new review in the {nameof(CreateReview)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		[HttpPut("{userId}/{productId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateReview(string userId, string productId, [FromBody] ReviewUpdateDto reviewDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateReview)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			if (userId != reviewDto.UserId || productId != reviewDto.ProductId) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateReview)}. The user id and product id in the request body do not match the ones in the request url.");
				
				return BadRequest($"The user id and product id in the request body do not match the ones in the request url.");
			}
			
			try {
				var review = _mapper.Map<Review>(reviewDto);
				
				var exists = await _unitOfWork.Ratings.existsAsync(r => r.ProductId == productId && r.UserId == userId);
				
				if (!exists) {
					_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateReview)}. The user with id={userId} has not reviewed the product with product id={productId} before.");
					
					return BadRequest($"The user with id={userId} has not reviewed the product with product id={productId} before.");
				}
				
				review.Timestamp = DateTime.Now;
				
				_unitOfWork.Reviews.Update(review);
				await _unitOfWork.Save();
				
				return NoContent();
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to update a review in the {nameof(UpdateReview)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		[HttpDelete("{userId}/{productId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteReview(string userId, string productId) {
			if (userId == null || productId == null) {
				_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteReview)}. Either the user id or the product id is null.");
				
				return BadRequest(ModelState);
			}
			
			try {
				var exists = await _unitOfWork.Reviews.existsAsync(r => r.UserId == userId && r.ProductId == productId);
				
				if (!exists) {
					_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteReview)}. No reviews exist for the product ({productId}) and user ({userId}).");
					
					return NotFound($"No reviews exist for the product ({productId}) and user ({userId}).");
				}
				
				await _unitOfWork.Reviews.DeleteAsync(r => r.UserId == userId && r.ProductId == productId);
				await _unitOfWork.Save();
				
				return NoContent();
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to delete a review in the {nameof(DeleteReview)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
    }
}
