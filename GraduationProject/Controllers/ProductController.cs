using AutoMapper;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
		private readonly ILogger<ProductController> _logger;
		private readonly IMapper _mapper;

		public ProductController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<ProductController> logger, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
            _userManager = userManager;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllProducts()
		{
			try
			{
				var products = await _unitOfWork.Products.GetAllAsync();

				return Ok(_mapper.Map<IList<ProductDto>>(products));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong when trying to access all the products data.");
				return StatusCode(500, "Internal Server Error.");
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetProduct(string id)
		{
			try
			{
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == id);

				return Ok(_mapper.Map<ProductDto>(product));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the product with id={id}.");
				return StatusCode(500, "Internal Server Error.");
			}
		}

		[HttpGet]
		[Route("products/seed")]
		public async Task<IActionResult> SeedDatabase()
		{
			List<ProductDto> products = ProductSeeder.Seed(null);
			await _unitOfWork.Products.InsertRangeAsync(_mapper.Map<List<Product>>(products));
			// string output = "";
			// foreach (var product in products)
			// {
			// 	output += product.Title;
			// }

			await _unitOfWork.Save();
			return Ok("Data added successfully."); //  + output 
		}
	}
}
