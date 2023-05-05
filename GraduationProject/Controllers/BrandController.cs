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
    public class BrandController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<BrandController> _logger;
		private readonly IMapper _mapper;
		private readonly HashSet<string> _fields = new HashSet<string>(typeof(Brand).GetProperties().Select(p => p.Name));

		public BrandController(IUnitOfWork unitOfWork, ILogger<BrandController> logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}
        
		/// <summary>
		/// Get a page of brands.
		/// </summary>
		/// <param name="pagingFilter">The paging filter.</param>
		/// <param name="orderBy">The order by.</param>
		/// <returns></returns>
		/// <response code="200">Returns a page of brands.</response>
		/// <response code="500">If an error occurs.</response>
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Route("Brands")]
		public async Task<IActionResult> GetBrands([FromQuery] Categories_BrandsPagingFilter pagingFilter, [FromQuery] string? orderBy = null) {
			try {
				EntityFieldsFilter fieldsFilters = new EntityFieldsFilter() { OnlySelectFields = "Id,Name",  FieldsToExclude = "Products" };
				Expression<Func<Brand, Brand>> selectExpression = QueryableExtensions<Brand>.EntityFieldsSelector(fieldsFilters);
				
				var brands = await _unitOfWork.Brands.GetAllAsync(null, pagingFilter, true, selectExpression: selectExpression, orderBy: orderBy);
				
				var json = JsonConvert.SerializeObject(brands, Formatting.Indented,
					new GenericFieldBasedJsonConverter<Brand>(fieldsFilters.OnlySelectFields.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet(), fieldsFilters));
				
				return Ok(json);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the brands data.");
				return StatusCode(500, "Internal Server Error. Something went wrong when trying to access the brands data.");
			}
		}

        
		/// <summary>
		/// Get a brand by id.
		/// </summary>
		/// <param name="id">The brand id.</param>
		/// <returns></returns>
		/// <response code="200">Returns the brand with the specified id.</response>
		/// <response code="404">If the brand with the specified id is not found.</response>
		/// <response code="500">If an error occurs.</response>
        [HttpGet("{id}")]
		[ActionName(nameof(GetBrand))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetBrand(int id) {
			try {
				EntityFieldsFilter fieldsFilters = new EntityFieldsFilter() { OnlySelectFields = "Id,Name",  FieldsToExclude = "Products" };
				Expression<Func<Brand, Brand>> selectExpression = QueryableExtensions<Brand>.EntityFieldsSelector(fieldsFilters);
				
				var brand = await _unitOfWork.Brands.GetByAsync(p => p.Id == id, true, selectExpression: selectExpression);
				
				if (brand == null) {
					_logger.LogInformation($"Failed GET attempt in {nameof(GetBrand)}. Could not find a brand with id={id}.");
					
					return NotFound($"Could not find a brand with id={id}.");
				}
				
				var json = JsonConvert.SerializeObject(brand, Formatting.Indented,
					new GenericFieldBasedJsonConverter<Brand>(fieldsFilters.OnlySelectFields.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet(), fieldsFilters));
				
				return Ok(json);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the brand with id={id}.");
				return StatusCode(500, $"Internal Server Error. Something went wrong when trying to access the brand with id={id}.");
			}
		}
    }
}
