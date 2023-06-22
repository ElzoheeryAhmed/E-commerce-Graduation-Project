using System.Text;
using System.Linq.Expressions;
using AutoMapper;
using GraduationProject.Controllers.FilterParameters;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Repository.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase {
        private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ReviewController> _logger;
		private readonly IMapper _mapper;
		private readonly IHttpClientFactory _httpClientFactory;

		public ReviewController(IUnitOfWork unitOfWork, ILogger<ReviewController> logger, IMapper mapper, IHttpClientFactory httpClientFactory) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_httpClientFactory = httpClientFactory;
		}
        
		/// <summary>
		/// Get all reviews of a specific product.
		/// </summary>
		/// <param name="productId">The id of the product.</param>
		/// <param name="pagingFilter">The page number and page size.</param>
		/// <param name="orderBy">A comma-separated list of fields to order the results by. The results will be sorted in ascending order by default. To sort in descending order, prefix the field name with a hyphen (-).</param>
		/// <returns></returns>
		/// <response code="200">Returns the reviews of the specified product.</response>
		/// <response code="404">If the product is not found.</response>
		/// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetReviewsByProduct([FromQuery] string productId, [FromQuery] PagingFilter pagingFilter, [FromQuery] string? orderBy = null) {
			try {
				EntityFieldsFilter fieldsFilters = new EntityFieldsFilter() { OnlySelectFields = "Id,UserId,ReviewText,Timestamp,SentimentScore",  FieldsToExclude = "User,Product,ProductId" };
				Expression<Func<Review, Review>> selectExpression = QueryableExtensions<Review>.EntityFieldsSelector(fieldsFilters);
				
				var reviews = await _unitOfWork.Reviews.GetAllAsync(new List<Expression<Func<Review, bool>>>() { r => r.ProductId == productId }, pagingFilter, true, selectExpression: selectExpression, orderBy: orderBy);
				
				if (reviews == null) {
					_logger.LogWarning($"Failed GET attempt in {nameof(GetReviewsByProduct)}. Could not find a product with id={productId}.");
					
					return NotFound($"Could not find a product with id={productId}.");
				}

				return Ok(reviews);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access all the reviews of the specified product.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		
		/// <summary>
		/// Get all reviews of a specific user.
		/// </summary>
		/// <param name="userId">The id of the user.</param>
		/// <param name="productId">The id of the product.</param>
		/// <param name="pagingFilter">The page number and page size.</param>
		/// <param name="orderBy">A comma-separated list of fields to order the results by. The results will be sorted in ascending order by default. To sort in descending order, prefix the field name with a hyphen (-).</param>
		/// <returns></returns>
		/// <response code="200">Returns the reviews of the specified user.</response>
		/// <response code="404">If the user is not found.</response>
		/// <response code="500">If an internal server error occurs.</response>
        [HttpGet("{userId}/{productId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetProductReviewsByUser(string userId, string productId, [FromQuery] PagingFilter pagingFilter, [FromQuery] string? orderBy = null) {
			try {
				var reviews = await _unitOfWork.Reviews.GetAllAsync(
					new List<Expression<Func<Review, bool>>>() { r => r.UserId == userId && r.ProductId == productId },
					selectExpression: r => new Review { Id = r.Id, ReviewText = r.ReviewText, Timestamp = r.Timestamp , SentimentScore = r.SentimentScore }, orderBy: orderBy);
				
				if (reviews == null) {
					_logger.LogWarning($"Failed GET attempt in {nameof(GetProductReviewsByUser)}. Could not find any reviews for userId={userId} and productId={productId}.");
					
					return NotFound($"Could not find any reviews for userId={userId} and productId={productId}");
				}
				
				return Ok(_mapper.Map<List<ReviewDto>>(reviews));
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the review with userId={userId} and productId={productId}.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		/// <summary>
		/// Get a review by a review id.
		/// </summary>
		/// <param name="id">The review id.</param>
		/// <returns></returns>
		/// <response code="200">Returns the specified review.</response>
		/// <response code="404">If the review is not found.</response>
		/// <response code="500">If an internal server error occurs.</response>
        [HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetReview(int id) {
			try {
				var review = await _unitOfWork.Reviews.GetByAsync(
					r => r.Id == id,
					selectExpression: r => new Review { Id = r.Id, UserId = r.UserId, ProductId = r.ProductId, ReviewText = r.ReviewText, Timestamp = r.Timestamp, SentimentScore = r.SentimentScore });
				
				if (review == null) {
					_logger.LogWarning($"Failed GET attempt in {nameof(GetProductReviewsByUser)}. Could not find a review with id={id}.");
					
					return NotFound($"Failed GET attempt in {nameof(GetProductReviewsByUser)}. Could not find a review with id={id}.");
				}
				
				return Ok(review);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the review with id={id}.");
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		/// <summary>
		/// Create a new review.
		/// </summary>
		/// <param name="reviewDto">The review to create.</param>
		/// <returns></returns>
		/// <response code="201">Returns the newly created review.</response>
		/// <response code="400">If the review is not valid.</response>
		/// <response code="500">If an internal server error occurs.</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto reviewDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateReview)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			try {
				var review = _mapper.Map<Review>(reviewDto);
				
				// Create an anonymous object to hold the strings with their names.
				var requestBody = new
				{
					data = review.ReviewText
				};
				
				// Serialize the object to JSON.
				var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
				
				// Create the StringContent with JSON as the content.
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				
				// Make the POST request to the API
				var httpClient = _httpClientFactory.CreateClient();
				var response = await httpClient.PostAsync("https://ai.ap.ngrok.io/sentiment", content);
				
				// If the response is not successful, return an appropriate error response.
				if (!response.IsSuccessStatusCode) {
					return StatusCode((int)response.StatusCode, response.ReasonPhrase + ". The error occurred while sending a request to the 'sentiment' API.");
				}
				
				string responseContent = await response.Content.ReadAsStringAsync();
				
				review.SentimentScore = int.Parse(responseContent);
				
				await _unitOfWork.Reviews.InsertAsync(review);
				
				await _unitOfWork.Save();
				
				return CreatedAtAction(nameof(GetReview), new { Id = review.Id }, review);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to create a new review in the {nameof(CreateReview)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		/// <summary>
		/// Update a review.
		/// </summary>
		/// <param name="id">The review id.</param>
		/// <param name="reviewDto">The review to update.</param>
		/// <returns></returns>
		/// <response code="200">Returns the updated review.</response>
		/// <response code="400">If the review is not valid.</response>
		/// <response code="404">If the review is not found.</response>
		/// <response code="500">If an internal server error occurs.</response>
		[HttpPut("id")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewUpdateDto reviewDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateReview)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			try {
				var review = _mapper.Map<Review>(reviewDto);
				
				EntityFieldsFilter fieldsFilters = new EntityFieldsFilter() { OnlySelectFields = "Id,ReviewText,Timestamp,UserId,ProductId,SentimentScore",  FieldsToExclude = "User,Product" };
				Expression<Func<Review, Review>> selectExpression = QueryableExtensions<Review>.EntityFieldsSelector(fieldsFilters);
				
				var existingReview = await _unitOfWork.Reviews.GetByAsync(r => r.Id == id, selectExpression: selectExpression);
				
				if (existingReview == null) {
					_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateReview)}. Could not find any reviews with id={id}.");
					
					return BadRequest($"Invalid PUT attempt in {nameof(UpdateReview)}. Could not find any reviews with id={id}.");
				}
				
				existingReview.Timestamp = DateTime.Now;
				
				existingReview.ReviewText = review.ReviewText;
				
				// Create an anonymous object to hold the strings with their names.
				var requestBody = new
				{
					data = review.ReviewText
				};
				
				// Serialize the object to JSON.
				var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
				
				// Create the StringContent with JSON as the content.
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				
				// Make the POST request to the API
				var httpClient = _httpClientFactory.CreateClient();
				var response = await httpClient.PostAsync("https://ai.ap.ngrok.io/sentiment", content);
				
				// If the response is not successful, return an appropriate error response.
				if (!response.IsSuccessStatusCode) {
					return StatusCode((int)response.StatusCode, response.ReasonPhrase + ". The error occurred while sending a request to the 'sentiment' API.");
				}
				
				string responseContent = await response.Content.ReadAsStringAsync();
				
				existingReview.SentimentScore = int.Parse(responseContent);
				
				// Either use update, or don't use the selectExpression and the update will be done automatically.
				_unitOfWork.Reviews.Update(existingReview);
				
				await _unitOfWork.Save();
				
				return NoContent();
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to update a review in the {nameof(UpdateReview)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		/// <summary>
		/// Delete a review.
		/// </summary>
		/// <param name="id">The id of the review.</param>
		/// <returns></returns>
		/// <response code="204">If the review is deleted successfully.</response>
		/// <response code="400">If the review is not valid.</response>
		/// <response code="404">If the review is not found.</response>
		/// <response code="500">If an internal server error occurs.</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteReview(int id) {
			try {
				var exists = await _unitOfWork.Reviews.ExistsAsync(r => r.Id == id);
				
				if (!exists) {
					_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteReview)}. No reviews exist with id={id}.");
					
					return NotFound($"Invalid DELETE attempt in {nameof(DeleteReview)}. No reviews exist with id={id}.");
				}
				
				await _unitOfWork.Reviews.DeleteAsync(r => r.Id == id);
				await _unitOfWork.Save();
				
				return NoContent();
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to delete a review in the {nameof(DeleteReview)}.");
				
				return StatusCode(500, "Internal Server Error.");
			}
		}
		
		/// <summary>
		/// Apply sentiment analysis on a review text to get `1` if it was positive and `0` if it was negative.
		/// </summary>
		/// <param name="review">A string to apply sentiment analysis on.</param>
		/// <response code="200">Returns the sentiment score.</response>
		/// <response code="500">If an error occurs.</response>
		[HttpPost]
		public async Task<IActionResult> ApplySentimentAnalysis([FromBody] string review) {
			// Create an anonymous object to hold the strings with their names.
            var requestBody = new
            {
                data = review
            };
			
            // Serialize the object to JSON.
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
			
            // Create the StringContent with JSON as the content.
            var content = new StringContent(json, Encoding.UTF8, "application/json");
			
			// Make the POST request to the API
			var httpClient = _httpClientFactory.CreateClient();
			var response = await httpClient.PostAsync("https://ai.ap.ngrok.io/sentiment", content);
			
			// If the response is not successful, return an appropriate error response.
			if (!response.IsSuccessStatusCode) {
				return StatusCode((int)response.StatusCode, response.ReasonPhrase + ". The error occurred while sending a request to the 'sentiment' API.");
			}
			
			// If the response is successful, return the response content.
			string responseContent = await response.Content.ReadAsStringAsync();
			return Ok(responseContent);
		}
    }
}
