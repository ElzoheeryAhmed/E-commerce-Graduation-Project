using System.Linq;
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
    public class OrderController : ControllerBase
    {
        private AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet(template: "GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            
            var orders = await _context.Orders.Include(itm => itm.OrderItems).ToListAsync();
            List<OrderAdminDto> Odtos = new List<OrderAdminDto>();
            foreach (var order in orders)
            {
                var Odto = new OrderAdminDto();
                Odto.Id = order.Id;
                Odto.CustomerId = order.CustomerId;
                Odto.Status = order.Status.ToString();
                Odto.OrderDate = order.OrderDate;
                Odto.ReceiptDate = order.ReceiptDate;
                Odto.ShippingAddress = order.ShippingAddress;
                Odto.OrderItems = new List<OrderItemDetailsDto>();

                foreach (var orderItem in order.OrderItems)
                {
                    var OItemdto = new OrderItemDetailsDto();

                    OItemdto.ProductId = orderItem.ProductId;
                    OItemdto.Title = orderItem.Product.Title;
                    OItemdto.HighResImageURLs = orderItem.Product.HighResImageURLs;
                    OItemdto.Price = orderItem.Product.Price;
                    OItemdto.Quantity = orderItem.Quantity;

                    Odto.OrderItems.Add(OItemdto);
                }

                Odtos.Add(Odto);

            }

            return Ok(Odtos);
        }

        [HttpGet(template: "GetbyId/{id}")]
        public async Task<IActionResult> GetOrdersAsync(string id)
        {
            //Validation of CustomerId
            var isValidCustomer = await _context.Users.AnyAsync(i => i.Id == id);
            if (!isValidCustomer)
            {
                return BadRequest(error: "Invalid customer Id !");
            }

            var orders=await  _context.Orders.Where(c=>c.CustomerId==id).Include(itm=>itm.OrderItems).ToListAsync();
            List<OrderDetailsDto> Odtos=new List<OrderDetailsDto>(); 
            foreach (var order in orders)
            {
                var Odto =new OrderDetailsDto();
                Odto.Id = order.Id; 
                Odto.Status = order.Status.ToString(); 
                Odto.OrderDate=order.OrderDate; 
                Odto.ReceiptDate=order.ReceiptDate;
                Odto.ShippingAddress = order.ShippingAddress;
                Odto.OrderItems = new List<OrderItemDetailsDto>();

                foreach (var orderItem in order.OrderItems)
                {
                    var OItemdto = new OrderItemDetailsDto();

                    OItemdto.ProductId=orderItem.ProductId;
                    OItemdto.Title = orderItem.Product.Title;
                    OItemdto.HighResImageURLs = orderItem.Product.HighResImageURLs;
                    OItemdto.Price  = orderItem.Product.Price;
                    OItemdto.Quantity = orderItem.Quantity;

                    Odto.OrderItems.Add(OItemdto);
                }

                Odtos.Add(Odto);

            }

            return Ok(Odtos);    
        }

        [HttpPost]
        public async  Task<IActionResult> CreateOrderAsync([FromBody] OrderDto dto)
        {

            //Validation of CustomerId
            var isValidCustomer = await _context.Users.AnyAsync(i => i.Id == dto.CustomerId);
            if(!isValidCustomer) {
                return BadRequest(error: "Invalid customer Id !");
            }

            //Validation of products Id
            foreach (OrderItemDto OrderItem in dto.OrderItems)
            {
                var isValidProduct = await _context.Products.AnyAsync(i => i.Id == OrderItem.ProductId);
                if (!isValidProduct)
                {
                    return BadRequest(error: $"Invalid Product Id:{OrderItem.ProductId}");
                }

            }

            //Order to orderDto mapping
            var order = new Order() { CustomerId = dto.CustomerId ,ShippingAddress=dto.ShippingAddress};
            
            foreach (OrderItemDto OrderItem in dto.OrderItems) {
                order.OrderItems.Add(new OrderItem() { ProductId=OrderItem.ProductId,Quantity= OrderItem.Quantity });
           
            }

            //Adding to database
            await _context.Orders.AddAsync(order);
            _context.SaveChanges();

            return Ok(order);
           
        }

        [HttpPut(template: "ShippingAddress/{id}")]
        public async Task<IActionResult> UpdateShippingAddressAsync(int id, [FromBody] string shippingAddress)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound("Order is not found");
            }

            if (order.Status != OrderStatus.Confirmed) {
                return BadRequest($"Order is already {order.Status} can`t change the shipping address");
            }

            order.ShippingAddress = shippingAddress;

            //Adding to database
            _context.Orders.Update(order);
            _context.SaveChanges();



            return Ok(order);

        }
       
        [HttpPut(template: "Ship/{id}")]
        public async Task<IActionResult> ShipOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound($"Order is not found");
            }

            if (order.Status != OrderStatus.Confirmed)
            {
                return BadRequest($"Order is {order.Status.ToString()} can`t mark it shipped");

            }

            else { order.Status = OrderStatus.Shipped; }

            //Adding to database
            _context.Orders.Update(order);
            _context.SaveChanges();



            return Ok(order);

        }

        [HttpPut(template: "Receipt/{id}")]
        public async Task<IActionResult> ReceiptOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound($"Order is not found");
            }

            if (order.Status != OrderStatus.Shipped)
            {
                return BadRequest($"Order is {order.Status.ToString()} can`t mark it Receipted");

            }

            else { order.Status = OrderStatus.Receipted; }

            //Adding to database
            _context.Orders.Update(order);
            _context.SaveChanges();



            return Ok(order);

        }

        [HttpPut(template: "Cancel/{id}")]
        public async Task<IActionResult> CancelOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound($"Order is not found");
            }

            if ((order.Status == OrderStatus.Receipted) || (order.Status == OrderStatus.Returned))
            {
                return BadRequest($"Order is {order.Status.ToString()} can`t mark it Cancelled");

            }

            else { order.Status = OrderStatus.Cancelled; }

            //Adding to database
            _context.Orders.Update(order);
            _context.SaveChanges();



            return Ok(order);

        }

        [HttpPut(template: "return/{id}")  ]
        public async Task<IActionResult> returnOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound($"Order is not found");
            }

            if (order.Status != OrderStatus.Receipted)
            {
                return BadRequest($"Order is {order.Status.ToString()} can`t mark it Returned");

            }

            else { order.Status = OrderStatus.Returned; }

            //Adding to database
            _context.Orders.Update(order);
            _context.SaveChanges();



            return Ok(order);

        }

        
        

    }
}
