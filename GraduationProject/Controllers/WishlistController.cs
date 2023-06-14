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
        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet(template:"GetAll")]
        public async Task<IActionResult> GetAllasync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var wishlistNames = await _context.WishlistItems.Where(i => i.CustomerId == userId).Select(n => new { n.Name }).Distinct().ToListAsync();
            
           return Ok(wishlistNames);   
        }

        [HttpGet(template:"GetContent")]
        public async Task<IActionResult> GetContentasync([FromQuery] WishlistIdentifyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var wishlistItems = await _context.WishlistItems.Where(i => i.CustomerId == userId && i.Name == dto.Name).Include(i => i.Product)
                .Select(i => new  {
                    ProductId = i.ProductId,
                    Title = i.Product.Title,
                    HighResImageURLs = i.Product.HighResImageURLs,
                    Price = i.Product.Price,
                    Quantity=i.Quantity
                })
                .ToListAsync();

            //Validation of customer wishlist combination
            if (wishlistItems.Count ==0)
            {
                return NotFound();
            }

           

            return Ok(wishlistItems);
        }



        [HttpPost(template:"AddItem")]  
        public async Task<IActionResult> AddItemAsync([FromBody] WishlistItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            //Validation of products Id
            var isValidProduct = await _context.Products.AnyAsync(i => i.Id == dto.ProductId);
            if (!isValidProduct)
            {
                return BadRequest(error: $"Invalid Product Id:{dto.ProductId}");
            }

            var isExist = await _context.WishlistItems.AnyAsync(i => ((i.CustomerId == userId) && (i.ProductId == dto.ProductId)&& (i.Name == dto.Name)));
            if (isExist)
            {
                return Conflict("This item is exist before,only you can remove or update its quantity");
            }
            var wishlistItem = new WishlistItem();

            wishlistItem.Name = dto.Name;
            wishlistItem.CustomerId = userId;
            wishlistItem.ProductId= dto.ProductId;
            wishlistItem.Quantity=dto.Quantity; 



            await _context.WishlistItems.AddAsync(wishlistItem);
            _context.SaveChanges();

            return Ok(new {Name = wishlistItem.Name, ProductId = wishlistItem.ProductId, Quantity = wishlistItem.Quantity });
        }

        [HttpPut(template: "UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantityAsync([FromBody] WishlistItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var wishlistItem = await _context.WishlistItems.FindAsync(userId, dto.ProductId, dto.Name);
            
            if(wishlistItem is null)
            {
                return NotFound();
            }
            //Enity framework by default tarck changes on entity no need for update
            wishlistItem.Quantity = dto.Quantity;

            _context.SaveChanges();

            return Ok(new { Name = wishlistItem.Name, ProductId = wishlistItem.ProductId, Quantity = wishlistItem.Quantity });

        }

        [HttpDelete(template: "DeleteItem")]
        public async Task<IActionResult> DeleteItemAsync([FromBody] WishlistItemIdentifyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var wishlistItem = await _context.WishlistItems.FindAsync(userId, dto.ProductId, dto.Name);
            
            if(wishlistItem is null) {
                return NotFound();
            }

            _context.WishlistItems.Remove(wishlistItem);
            _context.SaveChanges(); 

            return Ok("Wishlist item is deleted successfully");    

        }
       
        [HttpDelete(template: "DeleteWishlist")]
        public async Task<IActionResult> DeleteWishlistAsync([FromBody] WishlistIdentifyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


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

    }
}
