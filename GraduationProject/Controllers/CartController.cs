using GraduationProject.Data;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]   
        public async Task<IActionResult> GetContentAsync(string id)
        {
            //Validation of CustomerId
            var isValidCustomer = await _context.Users.AnyAsync(i => i.Id == id);
            if (!isValidCustomer)
            {
                return BadRequest(error: "Invalid customer Id !");
            }


            var items = await _context.CartItems
                .Where(c=>c.CustomerId==id)
                .Include(p => p.Product)
                .Select(i=>new CartItemDetailsDto{
                    
                    ProductId = i.ProductId,
                    Title = i.Product.Title,
                    HighResImageURLs = i.Product.HighResImageURLs,
                    Price = i.Product.Price,
                    Quantity = i.Quantity

                })
                .ToListAsync();
           
            return Ok(items);

        }

        [HttpPost]
        public async Task<IActionResult> AddItemAsync( [FromBody] CartItemDto dto )
        {
            //Validation of CustomerId
            var isValidCustomer = await _context.Users.AnyAsync(i => i.Id == dto.CustomerId);
            if (!isValidCustomer)
            {
                return BadRequest(error: "Invalid customer Id !");
            }

            //Validation of products Id
            var isValidProduct = await _context.Products.AnyAsync(i => i.Id == dto.ProductId);
            if (!isValidProduct)
            {
                return BadRequest(error: $"Invalid Product Id:{dto.ProductId}");
            }

            var cartItem = new CartItem() {
                
                CustomerId = dto.CustomerId,
                ProductId=dto.ProductId,
                Quantity=dto.Quantity 
            
            };


            //Add to database 
            await _context.CartItems.AddAsync(cartItem);
            _context.SaveChanges();

            return Ok(cartItem);

        }


        [HttpPut]
        public async Task<IActionResult> UpdateQuantityAsync([FromBody] CartItemDto dto)
        {
            //Validation of CartItem
            var item = await _context.CartItems.FindAsync(dto.CustomerId, dto.ProductId);
            if (item is null)
            {
                return NotFound();
            }

            item.Quantity = dto.Quantity;


            //Add to database 
            _context.SaveChanges();

            return Ok(item); 
        }


        [HttpDelete(template: "DeleteItem")]
        public async Task<IActionResult> DeleteItemAsync([FromBody] CartItemIdentifyDto dto)
        {
            //Validation of CartItem
            var item = await _context.CartItems.FindAsync(dto.CustomerId, dto.ProductId);
            if (item is  null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(item);
            _context.SaveChanges();


            return Ok(item);
        }


        [HttpDelete(template: "DeleteAll")]
        public async Task<IActionResult> DeleteAllItemsAsync(string id)
        {
            var cartItems = await _context.CartItems.Where(i => i.CustomerId == id).ToListAsync();

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
