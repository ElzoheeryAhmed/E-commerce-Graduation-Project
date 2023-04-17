using AutoMapper;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Models.ModelEnums;
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

		public ProductController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<ProductController> logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
            _userManager = userManager;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetProducts([FromQuery] PagingFilter pagingFilter) {
			try {
				var products = await _unitOfWork.Products.GetPagedList(pagingFilter, null, null, new List<string> { "ProductCategories" });
				
				// This is not necessary as the ProductCategories property is already populated by the GetPagedList method.
				// foreach (var product in products) {
				// 	product.ProductCategories.AddRange((await _unitOfWork.ProductCategoryJoins.GetAllAsync(pcj => pcj.ProductId == product.Id)).Select(pcj => new ProductCategory() { Id = pcj.ProductCategoryId }).ToList());
				// }

				return Ok(_mapper.Map<IList<ProductDto>>(products));
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access all the products data.");
				return StatusCode(500, "Internal Server Error.");
			}
		}

		[HttpGet("{id}")]
		[ActionName(nameof(GetProduct))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetProduct(string id) {
			try {
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == id, new List<string> { "ProductCategories" });
				
				if (product == null) {
					_logger.LogWarning($"Failed GET attempt in {nameof(GetProduct)}. Could not find a product with id={id}.");
					
					return NotFound($"Could not find a product with id={id}.");
				}
				
				// This is not necessary as the ProductCategories property is already populated by the GetByAsync method.
				// var productCategoryJoin = await _unitOfWork.ProductCategoryJoins.GetAllAsync(pcj => pcj.ProductId == product.Id);
				// var productDtos = _mapper.Map<ProductDto>(product);
				// productDtos.ProductCategories = _mapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(
				// 	(await _unitOfWork.ProductCategories.GetAllAsync(pc => productCategoryJoin.Select(pcj => pcj.ProductCategoryId).Contains(pc.Id))).ToList());
				// product.ProductCategories = productCategoryJoin.Select(pcj => new ProductCategory() { Id = pcj.ProductCategoryId }).ToList();
				
				return Ok(_mapper.Map<ProductDto>(product));
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the product with id={id}.");
				return StatusCode(500, "Internal Server Error.");
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
			
			try {
				var product = _mapper.Map<Product>(productDto);
				
				// No need to check as the Id is auto-generated.
				// var exists = await _unitOfWork.Products.existsAsync(p => p.Id == product.Id);
				// if (exists) {
				// 	_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. A product with same key already exists.");
					
				// 	return BadRequest("A product with same key already exists.");
				// }
				
				product.Status = ProductStatus.Added;
				List<int> productCategoryIds = (await _unitOfWork.ProductCategories.GetAllAsync(pc => productDto.ProductCategories.Select(pcDto => pcDto.Name).Contains(pc.Name))).Select(pc => pc.Id).ToList();
				product.ProductCategories = null;
				await _unitOfWork.Products.InsertAsync(product);
				
				
				List<ProductCategoryJoin> productCategoryJoins = productCategoryIds.Select(pcId => new ProductCategoryJoin() { ProductId = product.Id, ProductCategoryId = pcId }).ToList();
				
				await _unitOfWork.ProductCategoryJoins.InsertRangeAsync(productCategoryJoins);
				// if (productCategoryIds != null && productCategoryIds.Count > 0)
				
				await _unitOfWork.Save();
				product.ProductCategories = productCategoryIds.Select(pcId => new ProductCategory() { Id = pcId }).ToList();
				return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to create a new product in the {nameof(CreateProduct)}.");
				
				return StatusCode(500, "Internal Server Error. Please try again later.");
			}
		}
		
		[HttpPut("{id}")]
		// [ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductUpdateDto productDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateProduct)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			try {
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == id);
				
				// var product = _mapper.Map<Product>(productDto);
				// var exists = await _unitOfWork.Products.existsAsync(p => p.Id == product.Id);
				
				if (product == null) {
					_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateProduct)}. A product with same key doesn't exist.");
					
					return NotFound("A product with same key doesn't exist.");
				}
				
				product.Status = ProductStatus.Updated;
				_mapper.Map(productDto, product);
				
				// Updating the existing product.
				// _unitOfWork.Products.Update(product);
				// await _unitOfWork.Save();
				// return NoContent();
				
				
				string currentproductId = product.Id;
				
				// This allows the database to generate a new Id.
				product.Id = null;
				
				product.DateAdded = DateTime.Now;

				// Creating a new product with the updated data and a new key.
				await _unitOfWork.Products.InsertAsync(product);
				string newProductId = product.Id; // Get the generated Id of the new product.
				
				await _unitOfWork.ProductUpdates.InsertAsync(new ProductUpdate {CurrentProductId = currentproductId, UpdatedProductId = newProductId});
				await _unitOfWork.Save();
				
				return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to update a product in the {nameof(UpdateProduct)}.");
				
				return StatusCode(500, "Internal Server Error. Please try again later.");
			}
		}
		
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteProduct(string id) {
			if (id == null) {
				_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteProduct)}. No id provided.");
				
				return BadRequest(ModelState);
			}
			
			try {
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == id);
				
				if (product == null) {
					_logger.LogInformation($"Invalid DELETE attempt in {nameof(DeleteProduct)}. A product with id={id} doesn't exist.");
					
					return NotFound($"A product with id={id} doesn't exist.");
				}
				
				product.Status = ProductStatus.Deleted;
				_unitOfWork.Products.Update(product);
				await _unitOfWork.Save();
				
				return NoContent();
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to delete a product in the {nameof(DeleteProduct)}.");
				
				return StatusCode(500, "Internal Server Error. Please try again later.");
			}
		}
	}
}
