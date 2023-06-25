using System.Security.Claims;
using GraduationProject.Data;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Controllers
{
    [Authorize(Roles ="User")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private AppDbContext _context;
        private readonly ILogger<UserController> _logger;

        public CartController(AppDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get user cart content 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns cart content of products</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpGet(template: "GetContent")]   
        public async Task<IActionResult> GetContentAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var items = await _context.CartItems
                    .Where(c => c.CustomerId == userId)
                    .Include(p => p.Product)
                    .Select(i => new
                    {

                        ProductId = i.ProductId,
                        Title = i.Product.Title,
                        HighResImageURLs = i.Product.HighResImageURLs,
                        Price = i.Product.Price,
                        Quantity = i.Quantity

                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during retrieving cart content of user with userId{userId}.");
                return StatusCode(500, "Internal Server Error.");
            }
        }

        /// <summary>
        /// Add product to user cart  
        /// </summary>
        /// <param name="dto"> Product with its quantity to be added into the cart</param> 
        /// <returns></returns>
        /// <response code="200">Product is added successfully to the cart</response>
        /// <response code="400">BadRequest, Invalid productId or Quantity</response>
        /// <response code="409">when the product is already exist in the cart</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpPost(template:"AddItem")]
        public async Task<IActionResult> AddItemAsync( [FromBody] CartItemDto dto )
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

                var isExist = await _context.CartItems.AnyAsync(i => ((i.CustomerId == userId) && (i.ProductId == dto.ProductId)));
                if (isExist)
                {
                    return Conflict("This item is exist before,only you can remove or update its quantity");
                }


                var cartItem = new CartItem()
                {

                    CustomerId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity

                };


                //Add to database 
                await _context.CartItems.AddAsync(cartItem);
                _context.SaveChanges();

                return Ok("Product is added successfully to the cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during adding product to user cart with userId:{userId} and ProductId:{dto.ProductId}.");
                return StatusCode(500, "Internal Server Error.");
            }
        }


        /// <summary>
        /// Update cartItem quantity  
        /// </summary>
        /// <param name="dto"> cartItem with its quantity to be updated </param> 
        /// <returns></returns>
        /// <response code="200">Product qunatity is updated successfully</response>
        /// <response code="400">BadRequest, Invalid productId or Quantity</response>
        /// <response code="404">Product is not exist in the user cart</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpPut(template:"UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantityAsync([FromBody] CartItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                //Validation of CartItem
                var item = await _context.CartItems
                    .Where(c => c.CustomerId == userId && c.ProductId == dto.ProductId)
                    .Include(p => p.Product).SingleOrDefaultAsync();
                if (item is null)
                {
                    return NotFound();
                }

                item.Quantity = dto.Quantity;


                //Add to database 
                _context.SaveChanges();

                return Ok(new
                {
                    ProductId = item.ProductId,
                    Title = item.Product.Title,
                    HighResImageURLs = item.Product.HighResImageURLs,
                    Price = item.Product.Price,
                    Quantity = item.Quantity

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during updating cartItem quantity in user cart with userId:{userId} and ProductId:{dto.ProductId}.");
                return StatusCode(500, "Internal Server Error.");
            }

        }


        /// <summary>
        /// Delete Item from the cart  
        /// </summary>
        /// <param name="ProductId"> Id of product to be deleted </param> 
        /// <returns></returns>
        /// <response code="200">Product is deleted successfully from the cart</response>
        /// <response code="400">BadRequest, Invalid productId</response>
        /// <response code="404">Product is not exist in the user cart</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpDelete(template: "DeleteItem")]
        public async Task<IActionResult> DeleteItemAsync([FromQuery] CartItemIdentifyDto ProductId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {

                //Validation of CartItem
                var item = await _context.CartItems.FindAsync(userId, ProductId);
                if (item is null)
                {
                    return NotFound();
                }

                _context.CartItems.Remove(item);
                _context.SaveChanges();


                return Ok("CartItem is successfully deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during deleting cartItem from user cart with userId:{userId} and ProductId:{ProductId}.");
                return StatusCode(500, "Internal Server Error.");
            }
        }


        /// <summary>
        /// Delete all Items from the cart  
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Now, The cart is empty</response>
        /// <response code="400">BadRequest, if the Cart is already empty</response>
        /// <response code="401">Unauthorized response, user is not authenticated</response>
        /// <response code="403">Forbidden response, user is not authorized to access this endpoint</response>
        /// <response code="500">Unexpected internal server error occured</response>
        [HttpDelete(template: "DeleteAll")]
        public async Task<IActionResult> DeleteAllItemsAsync()

        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            try
            {
                var cartItems = await _context.CartItems.Where(i => i.CustomerId == userId).ToListAsync();

                //Check cart content
                if (cartItems is null)
                {
                    return BadRequest(error: "Cart is already empty");
                }

                _context.CartItems.RemoveRange(cartItems);
                _context.SaveChanges();

                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Unexpected error occurs during deleting all items from user cart with userId:{userId}.");
                return StatusCode(500, "Internal Server Error.");
            }

        }

    }
}
