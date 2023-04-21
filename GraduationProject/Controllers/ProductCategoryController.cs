using AutoMapper;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ProductCategoryController> _logger;
		private readonly IMapper _mapper;
		private readonly HashSet<string> _fields = new HashSet<string>(typeof(ProductCategory).GetProperties().Select(p => p.Name));

		public ProductCategoryController(IUnitOfWork unitOfWork, ILogger<ProductCategoryController> logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}
        
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Route("ProductCategories")]
		public async Task<IActionResult> GetProductCategories([FromQuery] Categories_BrandsPagingFilter pagingFilter, [FromQuery] string? orderBy = null) {
			try {
				// if (pagingFilter.PageSize == 0) {
				// 	pagingFilter = null;
				// }
				
				var categories = await _unitOfWork.ProductCategories.GetAllAsync(null, pagingFilter, true, orderBy: orderBy);
				
				var json = JsonConvert.SerializeObject(categories, Formatting.Indented,
					new GenericFieldBasedJsonConverter<ProductCategory>(
						new HashSet<string>(typeof(ProductCategory).GetProperties().Select(p => p.Name)),
						null, "Products"));
				
				return Ok(json);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to the product categories data.");
				return StatusCode(500, "Internal Server Error. Something went wrong when trying to the product categories data.");
			}
		}
    }
}