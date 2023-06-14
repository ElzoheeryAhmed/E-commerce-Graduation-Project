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
        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet(template: "GetContent")]   
        public async Task<IActionResult> GetContentAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var items = await _context.CartItems
                .Where(c=>c.CustomerId== userId)
                .Include(p => p.Product)
                .Select(i=>new {
                    
                    ProductId = i.ProductId,
                    Title = i.Product.Title,
                    HighResImageURLs = i.Product.HighResImageURLs,
                    Price = i.Product.Price,
                    Quantity = i.Quantity

                })
                .ToListAsync();
           
            return Ok(items);

        }
        
        [HttpPost(template:"AddItem")]
        public async Task<IActionResult> AddItemAsync( [FromBody] CartItemDto dto )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            //Validation of products Id
            var isValidProduct = await _context.Products.AnyAsync(i => i.Id == dto.ProductId);
            if (!isValidProduct)
            {
                return BadRequest(error: $"Invalid Product Id:{dto.ProductId}");
            }

            var isExist = await _context.CartItems.AnyAsync(i=>((i.CustomerId == userId) && (i.ProductId==dto.ProductId)));
            if (isExist) {
                return Conflict("This item is exist before,only you can remove or update its quantity");
            }


            var cartItem = new CartItem() {
                
                CustomerId = userId,
                ProductId=dto.ProductId,
                Quantity=dto.Quantity 
            
            };


            //Add to database 
            await _context.CartItems.AddAsync(cartItem);
            _context.SaveChanges();

            return Ok("Product is added successfully to the cart");

        }

        [HttpPut(template:"UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantityAsync([FromBody] CartItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


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

            return Ok(new {
                ProductId = item.ProductId,
                Title = item.Product.Title,
                HighResImageURLs = item.Product.HighResImageURLs,
                Price = item.Product.Price,
                Quantity = item.Quantity

            }); 
        }

        
        [HttpDelete(template: "DeleteItem")]
        public async Task<IActionResult> DeleteItemAsync([FromBody] CartItemIdentifyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            //Validation of CartItem
            var item = await _context.CartItems.FindAsync(userId, dto.ProductId);
            if (item is  null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(item);
            _context.SaveChanges();


            return Ok("CartItem is successfully deleted");
        }

        [HttpDelete(template: "DeleteAll")]
        public async Task<IActionResult> DeleteAllItemsAsync()

        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var cartItems = await _context.CartItems.Where(i => i.CustomerId == userId).ToListAsync();

            //Check cart content
            if (cartItems is null)
            {
                return BadRequest(error:"Cart is already empty");
            }

            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            return Ok(cartItems);

        }

    }
}
