using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using GraduationProject.Controllers.Helpers;
using GraduationProject.Data;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles= "Admin")]
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
        [Authorize(Roles = "User")]
        [HttpGet(template: "GetMyOrders")]
        public async Task<IActionResult> GetMyOrdersAsync()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var orders = await _context.Orders
                .Where(c => c.CustomerId == userId)
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
        
        [Authorize(Roles = "User")]
        [HttpPost(template:"AddOrder")]
        public async  Task<IActionResult> CreateOrderAsync([FromBody] OrderDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
            var order = new Order() { CustomerId = userId , ShippingAddress=dto.ShippingAddress};
            order.OrderItems = new List<OrderItem>();
            foreach (OrderItemDto OrderItem in dto.OrderItems) {
                order.OrderItems.Add(new OrderItem() { ProductId=OrderItem.ProductId,Quantity= OrderItem.Quantity });
           
            }

            //Adding to database
            await _context.Orders.AddAsync(order);
            _context.SaveChanges();

            return Ok(order);
           
        }
        
        [Authorize(Roles = "User")]
        [HttpPut(template: "UpdateShippingAddress")]
        public async Task<IActionResult> UpdateShippingAddressAsync([FromBody] OrderShippingDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var order = await _context.Orders.FindAsync(dto.OrderId);

            if (order == null)
            {
                return NotFound("Order is not found");
            }

            //check that this order belongs to the loginned customer
            if (order.CustomerId!= userId)
            {
                return Unauthorized();
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
       

        [Authorize(Roles = "Admin")]
        [HttpPut(template: "ChangeStatus")  ]
        public async Task<IActionResult> ChangeStatusAsync([FromQuery][Required]int orderId, [FromQuery][Required] OrderStatus reqStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound($"Order is not found");
            }

            var OStatusModel= OrderHelper.changeStatus(order.Status, reqStatus);


            if (!OStatusModel.IsSucceeded)
            {
                return BadRequest(OStatusModel.Message);

            }

            else { order.Status = OStatusModel.newStatus; }

            //Adding to database
            _context.SaveChanges();

            return Ok(order);

        }

        
        

    }
}
