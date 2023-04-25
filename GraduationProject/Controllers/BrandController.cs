using AutoMapper;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Repository;
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
        
        /*[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Route("Brands")]
		public async Task<IActionResult> GetBrands([FromQuery] Categories_BrandsPagingFilter pagingFilter, [FromQuery] string? orderBy = null) {
			try {
				var brands = await _unitOfWork.Brands.GetAllAsync(null, pagingFilter, true, orderBy: orderBy);
				
				var json = JsonConvert.SerializeObject(brands, Formatting.Indented,
					new GenericFieldBasedJsonConverter<Brand>(
						new HashSet<string>(typeof(Brand).GetProperties().Select(p => p.Name)),
						null, "Products"));
				
				return Ok(json);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the brands data.");
				return StatusCode(500, "Internal Server Error. Something went wrong when trying to access the brands data.");
			}
		}
		*/
        /*
        [HttpGet("{id}")]
		[ActionName(nameof(GetBrand))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetBrand(int id) {
			try {
				var brand = await _unitOfWork.Brands.GetByAsync(p => p.Id == id, true);
				
				if (brand == null) {
					_logger.LogInformation($"Failed GET attempt in {nameof(GetBrand)}. Could not find a brand with id={id}.");
					
					return NotFound($"Could not find a brand with id={id}.");
				}
				
				var json = JsonConvert.SerializeObject(brand, Formatting.Indented,
					new GenericFieldBasedJsonConverter<Brand>(_fields, fieldsToExclude: "Products"));
				
				return Ok(json);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the product with id={id}.");
				return StatusCode(500, $"Internal Server Error. Something went wrong when trying to access the data of the product with id={id}.");
			}
		}*/
    }
}