using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
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

            var orders = await _context.Orders
                .Include(itm => itm.OrderItems)
                .ThenInclude(p => p.Product)
                .Select(o=>new OrderAdminDto
                {
                    Id=o.Id,    
                    CustomerId=o.CustomerId,
                    Status=o.Status.ToString(),
                    OrderDate=o.OrderDate,
                    ReceiptDate=o.ReceiptDate,
                    ShippingAddress=o.ShippingAddress,
                    OrderItems= o.OrderItems.Select(i=>new OrderItemDetailsDto
                    {
                        ProductId=i.ProductId,
                        Title=i.Product.Title,
                        HighResImageURLs=i.Product.HighResImageURLs,
                        Price= i.Product.Price,
                        Quantity = i.Quantity

                    }).ToList() 
                } )
                .ToListAsync();
            /*List<OrderAdminDto> Odtos = new List<OrderAdminDto>();
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
            */
            return Ok(orders);
        }

        [HttpGet(template: "GetbyId/{customerId}")]
        public async Task<IActionResult> GetOrdersAsync(string customerId)
        {
            //Validation of CustomerId
            var isValidCustomer = await _context.Users.AnyAsync(i => i.Id == customerId);
            if (!isValidCustomer)
            {
                return BadRequest(error: "Invalid customer Id !");
            }

            var orders = await _context.Orders
                .Where(c => c.CustomerId == customerId)
                .Include(itm => itm.OrderItems)
                .ThenInclude(p => p.Product)
                .Select(o => new OrderDetailsDto
                {
                    Id = o.Id,
                    Status = o.Status.ToString(),
                    OrderDate = o.OrderDate,
                    ReceiptDate = o.ReceiptDate,
                    ShippingAddress = o.ShippingAddress,
                    OrderItems = o.OrderItems.Select(i => new OrderItemDetailsDto
                    {
                        ProductId = i.ProductId,
                        Title = i.Product.Title,
                        HighResImageURLs = i.Product.HighResImageURLs,
                        Price = i.Product.Price,
                        Quantity = i.Quantity

                    }).ToList()
                })
                .ToListAsync();
            /*List<OrderDetailsDto> Odtos=new List<OrderDetailsDto>(); 
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
            */

            return Ok(orders);    
        }

        [HttpPost(template:"AddOrder")]
        public async  Task<IActionResult> CreateOrderAsync([FromBody] OrderDto dto)
        {

            //Validation of CustomerId
            var isValidCustomer = await _context.Users.AnyAsync(i => i.Id == dto.CustomerId);
            if(!isValidCustomer) {
                return BadRequest(error: "Invalid customer Id !");
            }

            dto.OrderItems=dto.OrderItems.OrderBy(i => i.ProductId).ToList();
            string prevProduct=string.Empty;
            //Validation of products Id
            foreach (OrderItemDto OrderItem in dto.OrderItems)
            {
                if((prevProduct!=string.Empty )&&(OrderItem.ProductId== prevProduct))
                {
                    return BadRequest(error: $"Invalid duplicate Products with Id:{prevProduct}");

                }
                var isValidProduct = await _context.Products.AnyAsync(i => i.Id == OrderItem.ProductId);
                if (!isValidProduct)
                {
                    return BadRequest(error: $"Invalid Product Id:{OrderItem.ProductId}");
                }
                prevProduct = OrderItem.ProductId;
               
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

        [HttpPut(template: "UpdateShippingAddress")]
        public async Task<IActionResult> UpdateShippingAddressAsync([FromBody] OrderShippingDto dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);

            if (order == null)
            {
                return NotFound("Order is not found");
            }

            //if order overtakes confirmed state you can`t change its states
            if (order.Status != OrderStatus.Confirmed) {
                return BadRequest($"Order is already {order.Status} can`t change the shipping address");
            }

            order.ShippingAddress = dto.ShippingAddress;

            //Adding to database
            _context.SaveChanges();



            return Ok(order);

        }
       
        [HttpPut(template: "Ship/{orderId}")]
        public async Task<IActionResult> ShipOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

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
            _context.SaveChanges();



            return Ok(order);

        }

        [HttpPut(template: "Receipt/{orderId}")]
        public async Task<IActionResult> ReceiptOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound($"Order is not found");
            }
            if (order.Status == OrderStatus.Receipted)
            {
                return BadRequest($"Order is already Receipted");

            }
            if (order.Status != OrderStatus.Shipped)
            {
                return BadRequest($"Order is {order.Status.ToString()} can`t mark it Receipted");

            }

            else { order.Status = OrderStatus.Receipted; }

            //Adding to database
            _context.SaveChanges();



            return Ok(order);

        }

        [HttpPut(template: "Cancel/{orderId}")]
        public async Task<IActionResult> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

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
            _context.SaveChanges();



            return Ok(order);

        }

        [HttpPut(template: "Return/{orderId}")  ]
        public async Task<IActionResult> ReturnOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

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
            _context.SaveChanges();



            return Ok(order);

        }

        
        

    }
}
