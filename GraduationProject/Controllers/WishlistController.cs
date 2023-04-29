using System.Xml.Linq;
using GraduationProject.Data;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {

        private AppDbContext _context;
        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet(template:"GetAll/{id}")]
        public async Task<IActionResult> GetAllasync(string id)
        {
            //Validation of CustomerId
            var isValidCustomer = await _context.Users.AnyAsync(i => i.Id == id);
            if (!isValidCustomer)
            {
                return BadRequest(error: "Invalid customer Id !");
            }

            var wishlistsNames = await _context.WishlistItems.Where(i => i.CustomerId == id).Select(n => new { n.Name }).Distinct().ToListAsync();
            
           return Ok(wishlistsNames);   
        }

        [HttpGet(template:"GetContent")]
        public async Task<IActionResult> GetContentasync([FromBody]string id,[FromBody]string name)
        {
            var wishlistItems = await _context.WishlistItems.Where(i => i.CustomerId == id && i.Name == name).Include(i => i.Product)
                .Select(i => new WishlistItemDetailsDto {
                    ProductId = i.ProductId,
                    Title = i.Product.Title,
                    HighResImageURLs = i.Product.HighResImageURLs,
                    Price = i.Product.Price,
                    Quantity=i.Quantity
                })
                .ToListAsync();

            //Validation of customer wishlist combination
            if (wishlistItems is null)
            {
                return NotFound();
            }

           

            return Ok(wishlistItems);
        }



        [HttpPost(template:"AddItem")]  
        public async Task<IActionResult> AddItemAsync([FromBody] WishlistItemDto dto)
        {

            //Validation of CustomerId
            var isValidCustomer = await _context.Users.AnyAsync(i => i.Id == dto.CustomerId);
            if (!isValidCustomer)
            {
                return BadRequest(error: $"Invalid customer Id {dto.CustomerId} !");
            }

            //Validation of products Id
            var isValidProduct = await _context.Products.AnyAsync(i => i.Id == dto.ProductId);
            if (!isValidProduct)
            {
                return BadRequest(error: $"Invalid Product Id:{dto.ProductId}");
            }
            
            var wishlistitem = new WishlistItem();

            wishlistitem.Name = dto.Name;
            wishlistitem.CustomerId = dto.CustomerId;
            wishlistitem.ProductId= dto.ProductId;
            wishlistitem.Quantity=dto.Quantity; 



            await _context.WishlistItems.AddAsync(wishlistitem);
            _context.SaveChanges();

            return Ok(wishlistitem);
        }

        [HttpPut(template: "UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantityAsync([FromBody] WishlistItemDto dto)
        {
            var wishlistItem = await _context.WishlistItems.FindAsync(dto.CustomerId, dto.ProductId, dto.Name);
            
            if(wishlistItem is null)
            {
                return NotFound();
            }
            //Enity framework by default tarck changes on entity no need for update
            wishlistItem.Quantity = dto.Quantity;

            _context.SaveChanges();

            return Ok(wishlistItem);

        }

        [HttpDelete(template: "DeleteItem")]
        public async Task<IActionResult> DeleteItemAsync([FromBody] WishlistItemIdentifyDto dto)
        {
            var wishlistItem = await _context.WishlistItems.FindAsync(dto.CustomerId, dto.ProductId, dto.Name);
            
            if(wishlistItem is null) {
                return NotFound();
            }

            _context.WishlistItems.Remove(wishlistItem);
            _context.SaveChanges(); 

            return Ok(wishlistItem);    

        }
       
        [HttpDelete(template: "DeleteWishlist")]
        public async Task<IActionResult> DeleteWishlistAsync([FromBody] string id,[FromBody] string name)
        {
            var wishlistItems = await _context.WishlistItems.Where(i => i.CustomerId == id && i.Name == name)
                .ToListAsync();
            
            //Validation of customer wishlist combination
            if (wishlistItems is null)
            {
                return NotFound();
            }

            _context.WishlistItems.RemoveRange(wishlistItems);
            _context.SaveChanges();

            return Ok(wishlistItems);   

        }

    }
}
