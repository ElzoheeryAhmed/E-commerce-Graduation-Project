using System.Linq.Expressions;
using AutoMapper;
using GraduationProject.Controllers.FilterParameters;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Repository.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase {
        private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<RatingController> _logger;
		private readonly IMapper _mapper;

		public RatingController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<RatingController> logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}
        
		/// <summary>
		/// Get all ratings of a product.
		/// </summary>
		/// <param name="id">The id of the product.</param>
		/// <param name="pagingFilter">The page number and page size.</param>
		/// <param name="orderBy">A comma-separated list of fields to order the results by. The results will be sorted in ascending order by default. To sort in descending order, prefix the field name with a hyphen (-).</param>
		/// <returns></returns>
		/// <response code="200">Returns the ratings of the specified product.</response>
		/// <response code="404">If the product does not exist.</response>
		/// <response code="500">If something went wrong on the server.</response>
        [HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRatingsByProduct(string id, [FromQuery] PagingFilter pagingFilter, [FromQuery] string? orderBy = null) {
			try {
				EntityFieldsFilter fieldsFilters = new EntityFieldsFilter() { OnlySelectFields = "UserId,RatingValue,Timestamp",  FieldsToExclude = "User,Product,ProductId" };
				Expression<Func<Rating, Rating>> selectExpression = QueryableExtensions<Rating>.EntityFieldsSelector(fieldsFilters);
				
				var ratings = await _unitOfWork.Ratings.GetAllAsync(new List<Expression<Func<Rating, bool>>>() { r => r.ProductId == id }, pagingFilter, true, selectExpression: selectExpression, orderBy: orderBy);
				
				if (ratings == null) {
					_logger.LogWarning($"Failed GET attempt in {nameof(GetRatingsByProduct)}. Could not find a product with id={id}.");
					
					return NotFound($"Could not find a product with id={id}.");
				}
				
				return Ok(ratings);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access all the ratings of the specified product.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
        
		/// <summary>
		/// Get the rating of a product given by a user.
		/// </summary>
		/// <param name="userId">The id of the user.</param>
		/// <param name="productId">The id of the product.</param>
		/// <returns>Returns the rating of the product given by the user.</returns>
		/// <response code="200">Returns the rating of the product given by the user.</response>
		/// <response code="404">If the rating does not exist.</response>
		/// <response code="500">If something went wrong on the server.</response>
        [HttpGet("{userId}/{productId}")]
		[ActionName(nameof(GetProductRatingByUser))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetProductRatingByUser(string userId, string productId) {
			try {
				var rating = await _unitOfWork.Ratings.GetByAsync(
					r => r.UserId == userId && r.ProductId == productId,
					selectExpression: r => new Rating { RatingValue = r.RatingValue, Timestamp = r.Timestamp });
				
				if (rating == null) {
					_logger.LogWarning($"Failed GET attempt in {nameof(GetProductRatingByUser)}. Could not find any ratings for userId={userId} and productId={productId}.");
					
					return NotFound($"Could not find any ratings with userId={userId} and productId={productId}");
				}
				
				return Ok(_mapper.Map<RatingDto>(rating));
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the ratings for userId={userId} and productId={productId}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		
		/// <summary>
		/// Create a new rating for a product.
		/// </summary>
		/// <param name="ratingDto">The rating to create.</param>
		/// <returns>Returns the created rating.</returns>
		/// <response code="201">Returns the created rating.</response>
		/// <response code="400">If the request body is not valid or the current user has already rated the same product before.</response>
		/// <response code="500">If something went wrong on the server.</response>
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
				
				var exists = await _unitOfWork.Ratings.ExistsAsync(r => r.ProductId == rating.ProductId && r.UserId == rating.UserId);
				
				if (exists) {
					_logger.LogInformation($"Invalid POST attempt in {nameof(CreateRating)}. The current user has already rated the same product before.");
					
					return BadRequest("The current user has already rated the same product before.");
				}
				
				await _unitOfWork.Ratings.InsertAsync(rating);
				
				// Updating the VoteAverage and VoteCount properties of the product.
				Product product = await _unitOfWork.Products.GetByAsync(p => p.Id == ratingDto.ProductId);
				
				product.VoteAverage = (product.VoteAverage*product.VoteCount + rating.RatingValue)/(product.VoteCount+1);
				product.VoteCount++;
				
				await _unitOfWork.Save();
				
				return CreatedAtAction(nameof(GetProductRatingByUser), new { userId = rating.UserId, productId = rating.ProductId }, rating);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to create a new rating in the {nameof(CreateRating)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		/// <summary>
		/// Update a rating.
		/// </summary>
		/// <param name="userId">The id of the user.</param>
		/// <param name="productId">The id of the product.</param>
		/// <param name="ratingDto">The rating to update.</param>
		/// <returns>Returns no content.</returns>
		/// <response code="200">The updated rating.</response>
		/// <response code="400">If the request body is not valid or the user id and product id in the request body do not match the ones in the request url.</response>
		/// <response code="404">If the rating does not exist.</response>
		/// <response code="500">If something went wrong on the server.</response>
		[HttpPut("{userId}/{productId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
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
				
				EntityFieldsFilter fieldsFilters = new EntityFieldsFilter() { OnlySelectFields = "RatingValue,Timestamp",  FieldsToExclude = "User,Product,UserId,ProductId" };
				Expression<Func<Rating, Rating>> selectExpression = QueryableExtensions<Rating>.EntityFieldsSelector(fieldsFilters);
				
				var existingRating = await _unitOfWork.Ratings.GetByAsync(r => r.ProductId == productId && r.UserId == userId, selectExpression: selectExpression);
				
				if (existingRating == null) {
					_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateRating)}. The user with id={userId} has not rated the product with product id={productId} before.");
					
					return NotFound($"The user with id={userId} has not rated the product with product id={productId} before.");
				}
				
				rating.Timestamp = DateTime.Now;
				
				// Updating the VoteAverage and VoteCount properties of the product.
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == productId);
				
				product.VoteAverage = (product.VoteAverage*product.VoteCount - existingRating.RatingValue + rating.RatingValue )/(product.VoteCount);
				
				_unitOfWork.Ratings.Update(rating);
				
				// _unitOfWork.Products.Update(product);
				
				await _unitOfWork.Save();
				
				return Ok(rating);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to update a rating in the {nameof(UpdateRating)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		/// <summary>
		/// Delete a rating.
		/// </summary>
		/// <param name="userId">The id of the user.</param>
		/// <param name="productId">The id of the product.</param>
		/// <returns>Returns no content.</returns>
		/// <response code="204">The rating deleted successfully.</response>
		/// <response code="400">If the user id or product id is null.</response>
		/// <response code="404">If the rating does not exist.</response>
		/// <response code="500">If something went wrong on the server.</response>
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
				EntityFieldsFilter fieldsFilters = new EntityFieldsFilter() { OnlySelectFields = "RatingValue",  FieldsToExclude = "User,Product,UserId,ProductId,Timestamp" };
				Expression<Func<Rating, Rating>> selectExpression = QueryableExtensions<Rating>.EntityFieldsSelector(fieldsFilters);
				
				var rating = await _unitOfWork.Ratings.GetByAsync(r => r.ProductId == productId && r.UserId == userId, selectExpression: selectExpression);
				
				// var exists = await _unitOfWork.Ratings.ExistsAsync(r => r.UserId == userId && r.ProductId == productId);
				
				if (rating == null) {
					_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteRating)}. The user with id={userId} has not rated the product with product id={productId} yet.");
					
					return NotFound($"The user with id={userId} has not rated the product with product id={productId} yet.");
				}
				
				await _unitOfWork.Ratings.DeleteAsync(r => r.UserId == userId && r.ProductId == productId);
				
				// Updating the VoteAverage and VoteCount properties of the product.
				Product product = await _unitOfWork.Products.GetByAsync(p => p.Id == productId);
				
				product.VoteAverage = (product.VoteAverage*product.VoteCount - rating.RatingValue)/(product.VoteCount - 1);
				product.VoteCount--;
				
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
