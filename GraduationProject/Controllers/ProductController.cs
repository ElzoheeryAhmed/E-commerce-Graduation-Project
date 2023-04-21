using System.Linq.Expressions;
using AutoMapper;
using GraduationProject.Controllers.Helpers;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Models.ModelEnums;
using GraduationProject.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GraduationProject.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ProductController> _logger;
		private readonly IMapper _mapper;
		private readonly HashSet<string> _fields = new HashSet<string>(typeof(Product).GetProperties().Select(p => p.Name));

		public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger, IMapper mapper) { // UserManager<User> userManager,
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}
		
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetProducts([FromQuery] PagingFilter pagingFilter, [FromQuery] string? onlyIncludeFields = null, [FromQuery] string? fieldsToExclude = null, [FromQuery] int? filterByBrand = null, [FromQuery] string? orderBy = null) {
			if (!string.IsNullOrWhiteSpace(onlyIncludeFields) && !string.IsNullOrWhiteSpace(fieldsToExclude)) {
                return BadRequest("Either use 'onlyIncludeFields' or 'fieldsToExclude', not both.");
            }
			
			try {
				List<string> entitiesToInclude = ProductHelper<Product>.GetNameOfEntitiesToInclude(onlyIncludeFields);
				
				
				List<Expression<Func<Product, bool>>> filterExpression = new List<Expression<Func<Product, bool>>>() {p => p.Status == ProductStatus.Current};
				
				if (filterByBrand != null && filterByBrand != 0) {
					filterExpression.Add(p => p.Brand.Id == filterByBrand);
				}
				
				
				var products = await _unitOfWork.Products.GetAllAsync( filterExpression, pagingFilter, entitiesToInclude.Count == 0, entitiesToInclude, orderBy);
				
				if (fieldsToExclude != null) {
					if (!fieldsToExclude.Contains("BrandId"))
						fieldsToExclude += ",BrandId";
				}
				
				else {
					fieldsToExclude = "BrandId";
				}
				
				// Does not return {Brand, Reviews, Ratings}
				var json = JsonConvert.SerializeObject(_mapper.Map<IList<ProductDtoWithBrand>>(products), Formatting.Indented,
					new GenericFieldBasedJsonConverter<ProductDtoWithBrand>(_fields, onlyIncludeFields, fieldsToExclude));
				
				return Ok(json);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access proudcts data.");
				return StatusCode(500, "Internal Server Error. Something went wrong when trying to access proudcts data.");
			}
		}

		[HttpGet("{id}")]
		[ActionName(nameof(GetProduct))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetProduct(string id, [FromQuery] string? onlyIncludeFields = null, [FromQuery] string? fieldsToExclude = null) {
			if (!string.IsNullOrWhiteSpace(onlyIncludeFields) && !string.IsNullOrWhiteSpace(fieldsToExclude)) {
                return BadRequest("Either use 'onlyIncludeFields' or 'fieldsToExclude', not both.");
            }
			
			try {
				
				List<string> entitiesToInclude = ProductHelper<Product>.GetNameOfEntitiesToInclude(onlyIncludeFields);
				
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == id, entitiesToInclude.Count == 0, entitiesToInclude);
				
				
				if (product == null) {
					_logger.LogInformation($"Failed GET attempt in {nameof(GetProduct)}. Could not find a product with id={id}.");
					
					return NotFound($"Could not find a product with id={id}.");
				}
				
				if (fieldsToExclude != null) {
					if (!fieldsToExclude.Contains("BrandId"))
						fieldsToExclude += ",BrandId";
				}
				
				else {
					fieldsToExclude = "BrandId";
				}
				
				var json = JsonConvert.SerializeObject(_mapper.Map<ProductDtoWithBrand>(product), Formatting.Indented,
					new GenericFieldBasedJsonConverter<ProductDtoWithBrand>(_fields, onlyIncludeFields, fieldsToExclude));
				
				return Ok(json);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the product with id={id}.");
				return StatusCode(500, $"Internal Server Error. Something went wrong when trying to access the data of the product with id={id}.");
			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			if (productDto.ProductCategories.Contains(0)) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. '0' cannot be used as a product category identifier.");
				
				return BadRequest("'0' cannot be used as a product category identifier.");
			}
			
			if (productDto.ProductCategories.Count != productDto.ProductCategories.Distinct().Count()) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. Duplicate product category identifiers are not allowed.");
				
				return BadRequest("Duplicate product category identifiers are not allowed.");
			}
			
			if (productDto.ProductCategories.Count == 0) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. Product category identifiers cannot be left empty.");
				
				return BadRequest("Product category identifiers cannot be left empty.");
			}
			
			if (productDto.BrandId == 0) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. Brand identifier cannot be left empty or take '0' as value.");
				
				return BadRequest("'0' cannot be passed as a brand identifier.");
			}
			
			try {
				var product = _mapper.Map<Product>(productDto);
				
				product.Status = ProductStatus.Added;
				
				await _unitOfWork.Products.InsertAsync(product);
				
				await _unitOfWork.Save();
				
				if (product == null) {
					_logger.LogWarning($"Failed POST attempt in {nameof(CreateProduct)}. Could not create a new product.");
					
					return BadRequest("Could not create a new product.");
				}
				
				var json = JsonConvert.SerializeObject(_mapper.Map<ProductDto>(product), Formatting.Indented,
					new GenericFieldBasedJsonConverter<ProductDto>(_fields, "", "VoteCount,VoteAverage,Ratings,Reviews"));
				
				return Ok(json);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to create a new product in the {nameof(CreateProduct)}.");
				
				return StatusCode(500, "Internal Server Error. Please try again later.");
			}
		}
		
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductUpdateDto productDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateProduct)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			if (productDto.ProductCategories.Contains(0)) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. '0' cannot be used as a product category identifier.");
				
				return BadRequest("'0' cannot be used as a product category identifier.");
			}
			
			if (productDto.ProductCategories.Count != productDto.ProductCategories.Distinct().Count()) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. Duplicate product category identifiers are not allowed.");
				
				return BadRequest("Duplicate product category identifiers are not allowed.");
			}
			
			// Request body cannot be an empty object.
			if (!DataValidationHelper.HasNonNullOrDefaultProperties(productDto)) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateProduct)}. The request body is empty.");
				
				return BadRequest("The request body is empty.");
			}
			
			try {
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == id, false, new List<string> { "ProductCategories.ProductCategory", "Brand" });
				
				// This is necessary to avoid the error "The property 'Product.Id' is part of a key and so cannot be modified or marked as modified. To change the principal of an existing entity with an identifying foreign key, first delete the dependent and invoke 'SaveChanges', and then associate the dependent with the new principal."
				// In other words, to prevent updating the existing product, we need to detach it from the context.
				_unitOfWork.Context.ChangeTracker.Clear();
				
				if (product == null) {
					_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateProduct)}. A product with same key doesn't exist.");
					
					return NotFound("A product with same key doesn't exist.");
				}
				
				// Changing the status of the new updated product entry.
				product.Status = ProductStatus.Updated;
				product.DateAdded = DateTime.Now;
				
				// Zero values are not ignored when mapping. To keep the existing product numeric value type data,
				// check if the productDto fields are zero, and if so, set them to the existing product data.
				ProductHelper<ProductUpdateDto>.KeepOriginalIfNewIsZero(productDto, product);
				
				// If the productDto does not contain any product categories, keep the existing product categories.
				if (productDto.ProductCategories.Count == 0)
					productDto.ProductCategories = product.ProductCategories.Select(pc => pc.ProductCategoryId).ToList();
				
				// Mapping the productDto to the product, overwriting existing fields. Any null fields in the productDto will be ignored when mapping.
				_mapper.Map(productDto, product);
				
				// Updating the existing product. The BL requires that any update request is added as a new product.
				// _unitOfWork.Products.Update(product);
				// await _unitOfWork.Save();
				// return NoContent();
				
				string currentproductId = product.Id;
				
				// This allows the database to generate a new Id.
				product.Id = null;
				
				// This allows the database to populate the Brand fields itself.
				product.Brand = null;
				
				// Creating a new product with the updated data and a new key.
				await _unitOfWork.Products.InsertAsync(product);
				string newProductId = product.Id; // Get the generated Id of the new product.
				
				// Inserting the product update record.
				await _unitOfWork.ProductUpdates.InsertAsync(new ProductUpdate {CurrentProductId = currentproductId, UpdatedProductId = newProductId});
				
				await _unitOfWork.Save();
				
				return CreatedAtAction(nameof(GetProduct), new { id = newProductId }, product);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to update a product in the {nameof(UpdateProduct)}.");
				
				return StatusCode(500, "Internal Server Error. Please try again later.");
			}
		}
		
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteProduct(string id) {
			if (id == null) {
				_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteProduct)}. No id provided.");
				
				return BadRequest(ModelState);
			}
			
			try {
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == id, false);
				
				if (product == null) {
					_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteProduct)}. A product with id={id} doesn't exist.");
					
					return NotFound($"A product with id={id} doesn't exist.");
				}
				
				product.Status = ProductStatus.Deleted;
				
				await _unitOfWork.Save();
				
				return Ok(product);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to delete a product in the {nameof(DeleteProduct)}.");
				
				return StatusCode(500, "Internal Server Error. Please try again later.");
			}
		}
	}
}
