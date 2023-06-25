using System.Security.Claims;
using System.Xml.Linq;
using GraduationProject.Data;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;

namespace GraduationProject.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {

        private AppDbContext _context;
        private readonly ILogger<UserController> _logger;

        public WishlistController(AppDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// Get all user wishlists names  
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns, list of wishlists names</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpGet(template:"GetAll")]
        public async Task<IActionResult> GetAllasync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var wishlistNames = await _context.WishlistItems.Where(i => i.CustomerId == userId).Select(n => new { n.Name }).Distinct().ToListAsync();

                return Ok(wishlistNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during retrieving user wishlists with userId:{userId}.");
                return StatusCode(500, "Internal Server Error.");
            }
        }


        /// <summary>
        /// Get user wishlist content  
        /// </summary>
        /// <param name="wishlistName">Wishlist name</param>
        /// <returns></returns>
        /// <response code="200">Returns, content of user wishlist</response>
        /// <response code="404">user has no wishlist with this name</response>
        /// <response code="400">BadRequest, name field is empty </response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpGet(template:"GetContent")]
        public async Task<IActionResult> GetContentasync([FromQuery] WishlistIdentifyDto wishlistName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var wishlistItems = await _context.WishlistItems.Where(i => i.CustomerId == userId && i.Name == wishlistName.Name).Include(i => i.Product)
                    .Select(i => new
                    {
                        ProductId = i.ProductId,
                        Title = i.Product.Title,
                        HighResImageURLs = i.Product.HighResImageURLs,
                        Price = i.Product.Price,
                        Quantity = i.Quantity
                    })
                    .ToListAsync();

                //Validation of customer wishlist combination
                if (wishlistItems.Count == 0)
                {
                    return NotFound();
                }



                return Ok(wishlistItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during retrieving user wishlist content with userId: {userId} and wishlist name:{wishlistName.Name}.");
                return StatusCode(500, "Internal Server Error.");
            }
        }


        /// <summary>
        /// Add item to wishlist  
        /// </summary>
        /// <param name="dto">wishlist name and item information</param>
        /// <returns></returns>
        /// <response code="200">item is added successfully to the wishlist</response>
        /// <response code="409">item is already exist in the wishlist</response>
        /// <response code="400">BadRequest,input data is invalid</response>
        /// <response code="404">Wishlist is not existed </response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpPost(template:"AddItem")]  
        public async Task<IActionResult> AddItemAsync([FromBody] WishlistItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                //Validation of products Id
                var isValidProduct = await _context.Products.AnyAsync(i => i.Id == dto.ProductId);
                if (!isValidProduct)
                {
                    return BadRequest(error: $"Invalid Product Id:{dto.ProductId}");
                }

                var isExist = await _context.WishlistItems.AnyAsync(i => ((i.CustomerId == userId) && (i.ProductId == dto.ProductId) && (i.Name == dto.Name)));
                if (isExist)
                {
                    return Conflict("This item is exist before,only you can remove or update its quantity");
                }

                var isWishlistExist = await _context.WishlistItems.AnyAsync(i => ((i.CustomerId == userId) && (i.Name == dto.Name)));
                if (isWishlistExist)
                {
                    return NotFound($"This is no wishlist with this name:{dto.Name}");
                }


                var wishlistItem = new WishlistItem();

                wishlistItem.Name = dto.Name;
                wishlistItem.CustomerId = userId;
                wishlistItem.ProductId = dto.ProductId;
                wishlistItem.Quantity = dto.Quantity;



                await _context.WishlistItems.AddAsync(wishlistItem);
                _context.SaveChanges();

                return Ok(new { Name = wishlistItem.Name, ProductId = wishlistItem.ProductId, Quantity = wishlistItem.Quantity });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during adding Productitem to user wishlist with userId{userId} and wishlist name:{dto.Name} .");
                return StatusCode(500, "Internal Server Error.");
            }
        }

        /// <summary>
        /// Update the quantity of wishlist item  
        /// </summary>
        /// <param name="dto">wishlist item information</param>
        /// <returns></returns>
        /// <response code="200">Returns, updated item information</response>
        /// <response code="404">Wishlist item is not exist </response>
        /// <response code="400">BadRequest,input data is invalid</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpPut(template: "UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantityAsync([FromBody] WishlistItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var wishlistItem = await _context.WishlistItems.FindAsync(userId, dto.ProductId, dto.Name);

                if (wishlistItem is null)
                {
                    return NotFound();
                }
                //Enity framework by default tarck changes on entity no need for update
                wishlistItem.Quantity = dto.Quantity;

                _context.SaveChanges();

                return Ok(new { Name = wishlistItem.Name, ProductId = wishlistItem.ProductId, Quantity = wishlistItem.Quantity });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during updating Productitem in user wishlist with userId{userId} and wishlist name:{dto.Name} .");
                return StatusCode(500, "Internal Server Error.");
            }

        }


        /// <summary>
        /// Delete wishlist item  
        /// </summary>
        /// <param name="dto">wishlist item information</param>
        /// <returns></returns>
        /// <response code="200">Returns, Wishlist item is deleted successfully</response>
        /// <response code="404">Wishlist item is not exist </response>
        /// <response code="400">BadRequest,input data is invalid</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpDelete(template: "DeleteItem")]
        public async Task<IActionResult> DeleteItemAsync([FromBody] WishlistItemIdentifyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var wishlistItem = await _context.WishlistItems.FindAsync(userId, dto.ProductId, dto.Name);

                if (wishlistItem is null)
                {
                    return NotFound();
                }

                _context.WishlistItems.Remove(wishlistItem);
                _context.SaveChanges();

                return Ok("Wishlist item is deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during deleting Productitem in user wishlist with userId{userId} and wishlist name:{dto.Name} .");
                return StatusCode(500, "Internal Server Error.");
            }
        }

        /// <summary>
        /// Delete wishlist  
        /// </summary>
        /// <param name="dto">wishlist name</param>
        /// <returns></returns>
        /// <response code="200">Returns, Wishlist of is deleted successfully</response>
        /// <response code="404">Wishlist item is not exist </response>
        /// <response code="400">BadRequest,input data is invalid</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpDelete(template: "DeleteWishlist")]
        public async Task<IActionResult> DeleteWishlistAsync([FromBody] WishlistIdentifyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var wishlistItems = await _context.WishlistItems.Where(i => i.CustomerId == userId && i.Name == dto.Name)
                    .ToListAsync();

                //Validation of customer wishlist combination
                if (wishlistItems.Count == 0)
                {
                    return NotFound();
                }

                _context.WishlistItems.RemoveRange(wishlistItems);
                _context.SaveChanges();

                return Ok($"Wishlist of name {dto.Name} is deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during deleting  wishlist with userId{userId} and wishlist name:{dto.Name} .");
                return StatusCode(500, "Internal Server Error.");
            }
        }

    }
}
