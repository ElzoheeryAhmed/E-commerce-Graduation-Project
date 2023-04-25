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
        public async Task<IActionResult> GetCartItemsbyIdAsync(string id)
        {
            //Validation of CustomerId
            var isValidCustomer = await _context.Users.AnyAsync(i => i.Id == id);
            if (!isValidCustomer)
            {
                return BadRequest(error: "Invalid customer Id !");
            }


            var items = await _context.CartItems.Where(c=>c.CustomerId==id).Include(p => p.Product).ToListAsync();
            List <CartItemDetailsDto> cartItemsdto=new List<CartItemDetailsDto> ();
            foreach (var item in items) { 
                var itemdto = new CartItemDetailsDto();   
                itemdto.ProductId=item.ProductId;   
                itemdto.Title=item.Product.Title;   
                itemdto.HighResImageURLs = item.Product.HighResImageURLs;   
                itemdto.Price=item.Product.Price;
                itemdto.Quantity = item.Quantity;

                cartItemsdto.Add(itemdto);

            }

            return Ok(cartItemsdto);

        }

        [HttpPost]
        public async Task<IActionResult> CreateCartItemAsync( [FromBody] CartItemDto dto )
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

            var cartItem = new CartItem() { CustomerId = dto.CustomerId, ProductId=dto.ProductId,Quantity=dto.Quantity };


            //Add to database 
            await _context.CartItems.AddAsync(cartItem);
            _context.SaveChanges();

            return Ok(cartItem);

        }


        [HttpPut]
        public async Task<IActionResult> UpdateQuantityAsync([FromBody] CartItemDto dto)
        {
            //Validation of CartItem
            var Item = await _context.CartItems.SingleOrDefaultAsync(i=>i.CustomerId==dto.CustomerId&&i.ProductId==dto.ProductId);
            if (Item==null)
            {
                return BadRequest(error: "Invalid cart item !");
            }

            Item.Quantity = dto.Quantity;


            //Add to database 
             _context.CartItems.Update(Item);
            _context.SaveChanges();

            return Ok(Item); 
        }


        [HttpDelete]
        public async Task<IActionResult> DeletecartItemAsync([FromBody] CartItemkeyDto dto)
        {
            //Validation of CartItem
            var Item = await _context.CartItems.SingleOrDefaultAsync(i => i.CustomerId == dto.CustomerId && i.ProductId == dto.ProductId);
            if (Item == null)
            {
                return BadRequest(error: "Invalid cart item !");
            }

            _context.CartItems.Remove(Item);
            _context.SaveChanges();


            return Ok(Item);
        }
    }
}
