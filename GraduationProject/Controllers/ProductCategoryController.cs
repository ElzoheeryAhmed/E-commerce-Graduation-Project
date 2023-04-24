using System.Linq.Expressions;
using AutoMapper;
using GraduationProject.Controllers.FilterParameters;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Repository.Extensions;
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
        
		
		/// <summary>
		/// Get a page of product categories.
		/// </summary>
		/// <param name="pagingFilter">The paging filter.</param>
		/// <param name="orderBy">The order by.</param>
		/// <returns></returns>
		/// <response code="200">Returns a page of product categories.</response>
		/// <response code="400">If the paging filter is invalid.</response>
		/// <response code="500">If an error occurs.</response>
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Route("ProductCategories")]
		public async Task<IActionResult> GetProductCategories([FromQuery] Categories_BrandsPagingFilter pagingFilter, [FromQuery] string? orderBy = null) {
			try {
				EntityFieldsFilter fieldsFilters = new EntityFieldsFilter() { OnlySelectFields = "Id,Name",  FieldsToExclude = "Products" };
				Expression<Func<ProductCategory, ProductCategory>> selectExpression = QueryableExtensions<ProductCategory>.EntityFieldsSelector(fieldsFilters);
				
				var categories = await _unitOfWork.ProductCategories.GetAllAsync(null, pagingFilter, true, selectExpression: selectExpression, orderBy: orderBy);
				
				var json = JsonConvert.SerializeObject(categories, Formatting.Indented,
					new GenericFieldBasedJsonConverter<ProductCategory>(fieldsFilters.OnlySelectFields.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet(), fieldsFilters));
				return Ok(json);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to the product categories data.");
				return StatusCode(500, "Internal Server Error. Something went wrong when trying to the product categories data.");
			}
		}
    }
}
