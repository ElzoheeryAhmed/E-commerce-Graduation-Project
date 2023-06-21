using System.Text;
using System.Linq.Expressions;
using AutoMapper;
using GraduationProject.Controllers.FilterParameters;
using GraduationProject.Controllers.Helpers;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Models.ModelEnums;
using GraduationProject.Repository.Extensions;

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
		private readonly IHttpClientFactory _httpClientFactory;

		public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger, IMapper mapper, IHttpClientFactory httpClientFactory) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_httpClientFactory = httpClientFactory;
		}
		
		/// <summary>
		/// Get a paged list of products, with filtering and ordering options.
		/// </summary>
		/// <param name="pagingFilter">The page number and page size.</param>
		/// <param name="fieldsFilters">A comma-separated list of fields to include and another one for fields to exclude from the results.</param>
		/// <param name="recordFilters">A collection of product filters.</param>
		/// <param name="orderBy">A comma-separated list of fields to order the results by. The results will be sorted in ascending order by default. To sort in descending order, prefix the field name with a hyphen (-).</param>
		/// <remarks>Fields can be filtered and/or ordered by passing a comma-separated fields.</remarks>
		/// <remarks>Does not return the product reviews and product ratings objects.</remarks>
		/// <returns></returns>
		/// <response code="200">Returns a paged list of products.</response>
		/// <response code="400">If the 'OnlyIncludeFields' and 'FieldsToExclude' properties are both set.</response>
		/// <response code="500">If an error occurs while trying to access the database.</response>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetProducts([FromQuery] PagingFilter pagingFilter, [FromQuery] ProductFieldsFilter fieldsFilters, [FromQuery] ProdcutRecordFilters recordFilters, [FromQuery] string? orderBy = null) {
			if (!string.IsNullOrWhiteSpace(fieldsFilters.OnlySelectFields) && !string.IsNullOrWhiteSpace(fieldsFilters.FieldsToExclude)) {
                return BadRequest("Either use 'OnlyIncludeFields' or 'FieldsToExclude', not both.");
            }
			
			try {
				// Removing the Reviews and Ratings entities from the list of entities to include, as they are not needed in this method and significantely increase the query time.
				fieldsFilters.FieldsToExclude += ",Reviews,Ratings";
				List<string> entitiesToInclude = ProductHelper<Product>.GetNameOfEntitiesToInclude(fieldsFilters);
				
				List<Expression<Func<Product, bool>>> filterExpression = new List<Expression<Func<Product, bool>>>() {p => p.Status == ProductStatus.Current};
				filterExpression.AddRange(ProductHelper.GetProductFilters(recordFilters));
				
				// Dynamically create a select expression based on the fields to include and exclude instead of returning all the fields from the Db and filter them afterwards.
				Expression<Func<Product, Product>> selectExpression = QueryableExtensions<Product>.EntityFieldsSelector(fieldsFilters);
				
				var products = await _unitOfWork.Products.GetAllAsync( filterExpression, pagingFilter, entitiesToInclude.Count == 0, entitiesToInclude, selectExpression, orderBy: orderBy);
				
				// Do not return {BrandId, Reviews, Ratings}.
				var json = JsonConvert.SerializeObject(_mapper.Map<IList<ProductDtoWithBrand>>(products), Formatting.Indented,
					new GenericFieldBasedJsonConverter<ProductDtoWithBrand>(_fields, fieldsFilters));
				
				return Ok(json);
				// return Ok(products);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access proudcts data.");
				return StatusCode(500, "Internal Server Error. Something went wrong when trying to access proudcts data.");
			}
		}

		/// <summary>
		/// Get a product by its id.
		/// </summary>
		/// <param name="id">The id of the product to get.</param>
		/// <param name="fieldsFilters">A comma-separated list of fields to include and another one for fields to exclude from the results.</param>
		/// <returns></returns>
		/// <response code="200">Returns the product with the specified id.</response>
		/// <response code="400">If the 'OnlyIncludeFields' and 'FieldsToExclude' properties are both set.</response>
		/// <response code="404">If the product with the specified id is not found.</response>
		[HttpGet("{id}")]
		[ActionName(nameof(GetProduct))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetProduct(string id, [FromQuery] ProductFieldsFilter fieldsFilters) {
			if (!string.IsNullOrWhiteSpace(fieldsFilters.OnlySelectFields) && !string.IsNullOrWhiteSpace(fieldsFilters.FieldsToExclude)) {
                return BadRequest("Either use 'OnlyIncludeFields' or 'FieldsToExclude', not both.");
            }
			
			try {
				List<string> entitiesToInclude = ProductHelper<Product>.GetNameOfEntitiesToInclude(fieldsFilters);
				
				// Dynamically create a select expression based on the fields to include and exclude instead of returning all the fields from the Db and filter them afterwards.
				Expression<Func<Product, Product>> selectExpression = QueryableExtensions<Product>.EntityFieldsSelector(fieldsFilters);
				
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == id, entitiesToInclude.Count == 0, entitiesToInclude, selectExpression);
				
				
				if (product == null) {
					_logger.LogInformation($"Failed GET attempt in {nameof(GetProduct)}. Could not find a product with id={id}.");
					
					return NotFound($"Could not find a product with id={id}.");
				}
				
				var json = JsonConvert.SerializeObject(_mapper.Map<ProductDtoWithBrand>(product), Formatting.Indented,
					new GenericFieldBasedJsonConverter<ProductDtoWithBrand>(_fields, fieldsFilters));
				
				return Ok(json);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access the data of the product with id={id}.");
				return StatusCode(500, $"Internal Server Error. Something went wrong when trying to access the data of the product with id={id}.");
			}
		}
		
		/// <summary>
		/// Create a new product.
		/// </summary>
		/// <param name="productDto">The product to create.</param>
		/// <returns></returns>
		/// <response code="201">Returns the newly created product.</response>
		/// <response code="400">If the product is null or if the given product model is invalid.</response>
		/// <response code="500">If there was an error on the server while creating the product.</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			if (productDto.ProductCategoriesIds.Contains(0)) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. '0' cannot be used as a product category identifier.");
				
				return BadRequest("'0' cannot be used as a product category identifier.");
			}
			
			if (productDto.ProductCategoriesIds.Count != productDto.ProductCategoriesIds.Distinct().Count()) {
				_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. Duplicate product category identifiers are not allowed.");
				
				return BadRequest("Duplicate product category identifiers are not allowed.");
			}
			
			if (productDto.ProductCategoriesIds.Count == 0) {
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
					new GenericFieldBasedJsonConverter<ProductDto>(_fields, new ProductFieldsFilter() { FieldsToExclude = "VoteCount,VoteAverage,Ratings,Reviews" }));
				
				return Ok(json);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to create a new product in the {nameof(CreateProduct)}.");
				
				return StatusCode(500, "Internal Server Error. Please try again later.");
			}
		}
		
		/// <summary>
		/// Update an existing product.
		/// </summary>
		/// <param name="id">The id of the product to update.</param>
		/// <param name="productDto">The product to update.</param>
		/// <returns></returns>
		/// <response code="200">Returns the updated product.</response>
		/// <response code="400">If the product is null or if the given product model is invalid.</response>
		/// <response code="404">If the product with the given id was not found.</response>
		/// <response code="500">If there was an error on the server while updating the product.</response>
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductUpdateDto productDto) {
			if (!ModelState.IsValid) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateProduct)}. The request body is not valid.");
				
				return BadRequest(ModelState);
			}
			
			if (productDto.ProductCategories != null) {
				if ( productDto.ProductCategories.Contains(0)) {
					_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. '0' cannot be used as a product category identifier.");
					
					return BadRequest("'0' cannot be used as a product category identifier.");
				}
				
				if (productDto.ProductCategories.Count != productDto.ProductCategories.Distinct().Count()) {
					_logger.LogInformation($"Invalid POST attempt in {nameof(CreateProduct)}. Duplicate product category identifiers are not allowed.");
					
					return BadRequest("Duplicate product category identifiers are not allowed.");
				}
			}
			
			// Request body cannot be an empty object.
			if (!DataValidationHelper.HasNonNullOrDefaultProperties(productDto)) {
				_logger.LogInformation($"Invalid PUT attempt in {nameof(UpdateProduct)}. The request body is empty.");
				
				return BadRequest("The request body is empty.");
			}
			
			try {
				// Dynamically create a select expression based on the fields to include and exclude instead of returning all the fields from the Db and filter them afterwards.
				ProductFieldsFilter fieldsFilters = new ProductFieldsFilter() { OnlySelectFields = "Id,Quantity,Title,Description,Price,Discount,Features,HighResImageURLs,BrandId,ProductCategories", FieldsToExclude = "VoteCount,VoteAverage,Ratings,Reviews,Status"};
				Expression<Func<Product, Product>> selectExpression = QueryableExtensions<Product>.EntityFieldsSelector(fieldsFilters);
				
				var product = await _unitOfWork.Products.GetByAsync(p => p.Id == id, false, new List<string> { "ProductCategories.ProductCategory" }, selectExpression);
				
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
				if (productDto.ProductCategories == null || productDto.ProductCategories.Count == 0)
					productDto.ProductCategories = product.ProductCategories.Select(pc => pc.ProductCategoryId).ToList();
				
				// Mapping the productDto to the product, overwriting existing fields. Any null fields in the productDto will be ignored when mapping.
				_mapper.Map(productDto, product);
				
				// This allows the database to generate a new Id and preventing it from adding a new product with the same id.
				product.Id = null;
				
				// This allows the database to populate the Brand fields itself and preventing it from adding a new brand with the same brand id to the brands table.
				product.Brand = null;
				
				// Creating a new product with the updated data and a new key.
				await _unitOfWork.Products.InsertAsync(product);
				string newProductId = product.Id; // Get the generated Id of the new product.
				
				// Inserting the product update record.
				await _unitOfWork.ProductUpdates.InsertAsync(new ProductUpdate {CurrentProductId = id, UpdatedProductId = newProductId});
				
				await _unitOfWork.Save();
				
				var json = JsonConvert.SerializeObject(product, Formatting.Indented,
					new GenericFieldBasedJsonConverter<Product>(fieldsFilters.OnlySelectFields.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet(), new ProductFieldsFilter()));
				
				return Ok(json);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to update a product in the {nameof(UpdateProduct)}.");
				
				return StatusCode(500, "Internal Server Error. Please try again later.");
			}
		}
		
		
		/// <summary>
		/// Deletes a product.
		/// </summary>
		/// <param name="id">The id of the product to delete.</param>
		/// <returns></returns>
		/// <response code="200">If the product was successfully deleted.</response>
		/// <response code="400">If the id is null or empty.</response>
		/// <response code="404">If the product with the specified id doesn't exist.</response>
		/// <response code="500">If there was an error on the server.</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
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
				// _unitOfWork.Products.Update(product);
				await _unitOfWork.Save();
				
				return Ok(product);
			}
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to delete a product in the {nameof(DeleteProduct)}.");
				
				return StatusCode(500, "Internal Server Error. Please try again later.");
			}
		}
		
		
		/// <summary>
		/// Get a paged list of product recommendations similar to a specific image, with field-filtering and ordering options.
		/// </summary>
		/// <param name="imageFile">An image to get product with similar featrues.</param>
		/// <param name="pagingFilter">The page number and page size.</param>
		/// <param name="fieldsFilters">A comma-separated list of fields to include and another one for fields to exclude from the results.</param>
		/// <param name="orderBy">A comma-separated list of fields to order the results by. The results will be sorted in ascending order by default. To sort in descending order, prefix the field name with a hyphen (-).</param>
		/// <remarks>Fields can be filtered and/or ordered by passing a comma-separated fields.</remarks>
		/// <remarks>Does not return the product reviews and product ratings objects.</remarks>
		/// <returns></returns>
		/// <response code="200">Returns a paged list of products.</response>
		/// <response code="400">If the 'OnlyIncludeFields' and 'FieldsToExclude' properties are both set.</response>
		/// <response code="500">If an error occurs while trying to access the database.</response>
		[HttpPost("recommendByImage")]
        public async Task<IActionResult> GetSimilarProducts(IFormFile imageFile, [FromQuery] PagingFilter pagingFilter, [FromQuery] ProductFieldsFilter fieldsFilters, [FromQuery] string? orderBy = null) {
			if (imageFile == null || imageFile.Length == 0) {
                return BadRequest("No image file provided.");
            }
			
			if (!string.IsNullOrWhiteSpace(fieldsFilters.OnlySelectFields) && !string.IsNullOrWhiteSpace(fieldsFilters.FieldsToExclude)) {
                return BadRequest("Either use 'OnlyIncludeFields' or 'FieldsToExclude', not both.");
            }
			
			using (var memoryStream = new MemoryStream()) {
				// Copy the image file into a memory stream
				await imageFile.CopyToAsync(memoryStream);
				memoryStream.Position = 0;
				
				// Create the HttpClient
				var httpClient = _httpClientFactory.CreateClient();
				
				// Create the MultipartFormDataContent
				var formData = new MultipartFormDataContent();
				
				// Create the HttpContent for the image file
				var fileContent = new StreamContent(memoryStream);
				fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") {
					Name = "file",
					FileName = imageFile.FileName
				};
				
				fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
				
				// Add the file content to the form data
				formData.Add(fileContent);
				
				// Send the POST request to the external API
				var response = await httpClient.PostAsync("https://ai.ap.ngrok.io/upload", formData);
				
				if (!response.IsSuccessStatusCode) {
					return StatusCode((int)response.StatusCode, response.ReasonPhrase + ". The error occurred while sending a request to the 'upload' API.");
				}
				
				// Deserialize the response from the external API
				var apiResponse = await response.Content.ReadAsStringAsync();
				var productIds = JsonConvert.DeserializeObject<List<string>>(apiResponse);
				
				// Query the database for product information based on the retrieved product IDs
				try {
					// Removing the Reviews and Ratings entities from the list of entities to include, as they are not needed in this method and significantely increase the query time.
					fieldsFilters.FieldsToExclude += ",Reviews,Ratings";
					List<string> entitiesToInclude = ProductHelper<Product>.GetNameOfEntitiesToInclude(fieldsFilters);
					
					List<Expression<Func<Product, bool>>> filterExpression = new List<Expression<Func<Product, bool>>>() { 
						p => p.Status == ProductStatus.Current,
						p => productIds.Contains(p.Id)
					};
					
					// Dynamically create a select expression based on the fields to include and exclude instead of returning all the fields from the Db and filter them afterwards.
					Expression<Func<Product, Product>> selectExpression = QueryableExtensions<Product>.EntityFieldsSelector(fieldsFilters);
					
					var products = await _unitOfWork.Products.GetAllAsync( filterExpression, pagingFilter, entitiesToInclude.Count == 0, entitiesToInclude, selectExpression, orderBy: orderBy);
				
					var json = JsonConvert.SerializeObject(_mapper.Map<IList<ProductDtoWithBrand>>(products), Formatting.Indented,
						new GenericFieldBasedJsonConverter<ProductDtoWithBrand>(_fields, fieldsFilters));
					
					return Ok(json);
				}
				
				catch (Exception ex) {
					_logger.LogError(ex, $"Something went wrong when trying to access proudcts data.");
					return StatusCode(500, "Internal Server Error. Something went wrong when trying to access proudcts data.");
				}
			}
        }
		
		
		/// <summary>
		/// Get a paged list of product recommendations for a user similar to a specific product, with field-filtering and ordering options.
		/// </summary>
		/// <param name="id">The product id to get similar recommendations for.</param>
		/// <param name="pagingFilter">The page number and page size.</param>
		/// <param name="fieldsFilters">A comma-separated list of fields to include and another one for fields to exclude from the results.</param>
		/// <param name="orderBy">A comma-separated list of fields to order the results by. The results will be sorted in ascending order by default. To sort in descending order, prefix the field name with a hyphen (-).</param>
		/// <remarks>Fields can be filtered and/or ordered by passing a comma-separated fields.</remarks>
		/// <remarks>Does not return the product reviews and product ratings objects.</remarks>
		/// <returns></returns>
		/// <response code="200">Returns a paged list of products.</response>
		/// <response code="400">If the 'OnlyIncludeFields' and 'FieldsToExclude' properties are both set.</response>
		/// <response code="404">If the product with the specified id is not found.</response>
		/// <response code="500">If an error occurs while trying to access the database.</response>
		[HttpPost("recommendByPid/{id}")]
		public async Task<IActionResult> RecommendByPid(string id, [FromQuery] PagingFilter pagingFilter, [FromQuery] ProductFieldsFilter fieldsFilters, [FromQuery] string? orderBy = null) {
			if (!string.IsNullOrWhiteSpace(fieldsFilters.OnlySelectFields) && !string.IsNullOrWhiteSpace(fieldsFilters.FieldsToExclude)) {
                return BadRequest("Either use 'OnlyIncludeFields' or 'FieldsToExclude', not both.");
            }
			
			var httpClient = _httpClientFactory.CreateClient();
			
			// Query the database for the product information
			var product = await _unitOfWork.Products.ExistsAsync(p => p.Id == id);
			
			if (product == null)
			{
				return NotFound($"A product with id={id} doesn't exist.");
			}
			
			// Create an anonymous object to hold the strings with their names
            var requestBody = new
            {
                data = id
            };

            // Serialize the object to JSON
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            // Create the StringContent with JSON as the content
            var content = new StringContent(json, Encoding.UTF8, "application/json");

			// Make the POST request to the API
			var response = await httpClient.PostAsync("https://ai.ap.ngrok.io/recomenByAsin", content);
			
			if (!response.IsSuccessStatusCode)
			{
				return StatusCode((int)response.StatusCode, response.ReasonPhrase + "Error occurred while sending a request to the 'recomenByAsin' API.");
			}
			
			// Deserialize the response from the external API
			var apiResponse = await response.Content.ReadAsStringAsync();
			
			var productIds = JsonConvert.DeserializeObject<List<string>>(apiResponse);
			
			// Query the database for product information based on the retrieved product IDs
			try {
				// Removing the Reviews and Ratings entities from the list of entities to include, as they are not needed in this method and significantely increase the query time.
				fieldsFilters.FieldsToExclude += ",Reviews,Ratings";
				List<string> entitiesToInclude = ProductHelper<Product>.GetNameOfEntitiesToInclude(fieldsFilters);
				
				List<Expression<Func<Product, bool>>> filterExpression = new List<Expression<Func<Product, bool>>>() { 
					p => p.Status == ProductStatus.Current,
					p => productIds.Contains(p.Id)
				};
				
				// Dynamically create a select expression based on the fields to include and exclude instead of returning all the fields from the Db and filter them afterwards.
				Expression<Func<Product, Product>> selectExpression = QueryableExtensions<Product>.EntityFieldsSelector(fieldsFilters);
				
				var products = await _unitOfWork.Products.GetAllAsync( filterExpression, pagingFilter, entitiesToInclude.Count == 0, entitiesToInclude, selectExpression, orderBy: orderBy);
			
				json = JsonConvert.SerializeObject(_mapper.Map<IList<ProductDtoWithBrand>>(products), Formatting.Indented,
					new GenericFieldBasedJsonConverter<ProductDtoWithBrand>(_fields, fieldsFilters));
				
				return Ok(json);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access proudcts data.");
				return StatusCode(500, "Internal Server Error. Something went wrong when trying to access proudcts data.");
			}
		}
		
		
		/// <summary>
		/// Get a paged list of relevant product recommendations for a user, with field-filtering and ordering options.
		/// </summary>
		/// <param name="id">The user id to get relevant recommendations for.</param>
		/// <param name="pagingFilter">The page number and page size.</param>
		/// <param name="fieldsFilters">A comma-separated list of fields to include and another one for fields to exclude from the results.</param>
		/// <param name="orderBy">A comma-separated list of fields to order the results by. The results will be sorted in ascending order by default. To sort in descending order, prefix the field name with a hyphen (-).</param>
		/// <remarks>Fields can be filtered and/or ordered by passing a comma-separated fields.</remarks>
		/// <remarks>Does not return the product reviews and product ratings objects.</remarks>
		/// <returns></returns>
		/// <response code="200">Returns a paged list of products.</response>
		/// <response code="400">If the 'OnlyIncludeFields' and 'FieldsToExclude' properties are both set.</response>
		/// <response code="500">If an error occurs while trying to access the database.</response>
		[HttpPost("recommendByUid/{id}")]
		public async Task<IActionResult> RecommendByUid(string id, [FromQuery] PagingFilter pagingFilter, [FromQuery] ProductFieldsFilter fieldsFilters, [FromQuery] string? orderBy = null) {
			if (!string.IsNullOrWhiteSpace(fieldsFilters.OnlySelectFields) && !string.IsNullOrWhiteSpace(fieldsFilters.FieldsToExclude)) {
                return BadRequest("Either use 'OnlyIncludeFields' or 'FieldsToExclude', not both.");
            }
			var httpClient = _httpClientFactory.CreateClient();
			
			// Create an anonymous object to hold the strings with their names.
            var requestBody = new
            {
                data = id
            };
			
            // Serialize the object to JSON.
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
			
            // Create the StringContent with JSON as the content.
            var content = new StringContent(json, Encoding.UTF8, "application/json");
			
			// Make the POST request to the API.
			var response = await httpClient.PostAsync("https://ai.ap.ngrok.io/recomenByUser", content);
			
			if (!response.IsSuccessStatusCode)
			{
				return StatusCode((int)response.StatusCode, response.ReasonPhrase + "Error occurred while sending a request to the 'recomenByUser' API.");
			}
			
			// Deserialize the response from the external API
			var apiResponse = await response.Content.ReadAsStringAsync();
			
			var productIds = JsonConvert.DeserializeObject<List<string>>(apiResponse);
			
			// Query the database for product information based on the retrieved product IDs
			try {
				// Removing the Reviews and Ratings entities from the list of entities to include, as they are not needed in this method and significantely increase the query time.
				fieldsFilters.FieldsToExclude += ",Reviews,Ratings";
				List<string> entitiesToInclude = ProductHelper<Product>.GetNameOfEntitiesToInclude(fieldsFilters);
				
				List<Expression<Func<Product, bool>>> filterExpression = new List<Expression<Func<Product, bool>>>() { 
					p => p.Status == ProductStatus.Current,
					p => productIds.Contains(p.Id)
				};
				
				// Dynamically create a select expression based on the fields to include and exclude instead of returning all the fields from the Db and filter them afterwards.
				Expression<Func<Product, Product>> selectExpression = QueryableExtensions<Product>.EntityFieldsSelector(fieldsFilters);
				
				var products = await _unitOfWork.Products.GetAllAsync(filterExpression, pagingFilter, entitiesToInclude.Count == 0, entitiesToInclude, selectExpression, orderBy: orderBy);
				
				json = JsonConvert.SerializeObject(_mapper.Map<IList<ProductDtoWithBrand>>(products), Formatting.Indented,
					new GenericFieldBasedJsonConverter<ProductDtoWithBrand>(_fields, fieldsFilters));
				
				return Ok(json);
			}
			
			catch (Exception ex) {
				_logger.LogError(ex, $"Something went wrong when trying to access proudcts data.");
				return StatusCode(500, "Internal Server Error. Something went wrong when trying to access proudcts data.");
			}
		}
	}
}
