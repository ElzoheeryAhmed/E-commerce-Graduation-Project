/*using System.Diagnostics;
using AutoMapper;
using EFCore.BulkExtensions;
using GraduationProject.IRepository;
using GraduationProject.Models;
using GraduationProject.Models.Dto;
using GraduationProject.Models.ModelEnums;
using GraduationProject.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DbSeederControlle : ControllerBase {
		private readonly int QUERY_TIMEOUT = 3600;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
		private readonly ILogger<DbSeederControlle> _logger;
		private readonly IMapper _mapper;
        
		public DbSeederControlle(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<DbSeederControlle> logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
            _userManager = userManager;
			_logger = logger;
			_mapper = mapper;
		}
        
        [HttpGet]
		[Route("users/seed")]
		public async Task<IActionResult> SeedUsers(int start, int end) {
            List<UserCreateDto> userDtos;
            Stopwatch sw;
            
			int file_index = start;
            while (file_index <= end) {
                Console.Write($"Seeding file {file_index}... ");
                
                sw = Stopwatch.StartNew();
                
                userDtos = UserSeeder.Seed(file_index++);
                
                foreach (var userDto in userDtos) {
                    User user = _mapper.Map<User>(userDto);
                    user.EmailConfirmed = true;
                    
                    var result = await _userManager.CreateAsync(user, "A1" + user.Email); // UserHelper.GeneratePassword(_userManager)
                    
                    if (!result.Succeeded) {
                        foreach (var error in result.Errors) {
                            _logger.LogError(error.Description);
                            ModelState.AddModelError("", error.Description);
                        }
                        
                        return Problem($"An error occurred while seeding user data.", statusCode: 500);
                    }
                }
		
                Console.WriteLine("Finished in " + sw.ElapsedMilliseconds/1000 + " sec");
            }
			return Ok("Data added successfully."); // + output );
		}
        
        
        [HttpGet]
        [Route("products/seed")]
        public async Task<IActionResult> SeedProducts() {
			Console.WriteLine("Parsing products data...");
            SeedProductDto seedProductDto = ProductSeeder.Seed(null, _mapper);
			
			Console.WriteLine("Inserting brands and product categories data into the Db...");
			var brands = seedProductDto.Brands.ToList();
			List<Brand> brandsList = new List<Brand>();
			
			for (int i = 0; i < seedProductDto.Brands.Count; i++) {
				brandsList.Add(new Brand() { Name = brands[i]});
			}
			
			BulkConfig bulkConfig = new BulkConfig() { BulkCopyTimeout = QUERY_TIMEOUT, SetOutputIdentity = true };
			await _unitOfWork.Context.BulkInsertAsync(brandsList, bulkConfig);
			await _unitOfWork.Context.BulkInsertAsync(seedProductDto.ProductCategories, bulkConfig);
			
			Console.WriteLine("Inserting products data into the Db...");
			List<Product> products = _mapper.Map<List<Product>>(seedProductDto.Products);
			products.ForEach(p => p.Status = ProductStatus.Current);
			// bulkConfig.IncludeGraph = true;
			await _unitOfWork.Context.BulkInsertAsync(products, bulkConfig);
			// bulkConfig.IncludeGraph = false;
			
			Console.WriteLine("Adding records to the ProductCategoryJoin table...");
			List<ProductCategoryJoin> productCategoryJoins = new List<ProductCategoryJoin>();
			
			for (int i = 0; i < products.Count; i++) {
				Console.Write($"Join record: {i}/{products.Count}\r");
				// products[i].ProductCategories.AddRange(seedProductDto.ProductCategories);
				for (int j = 0; j < seedProductDto.Products[i].ProductCategories.Count; j++) {
					ProductCategoryJoin productCategoryJoin = new ProductCategoryJoin() {ProductId=products[i].Id, ProductCategoryId=seedProductDto.Products[i].ProductCategories[j].Id};
					productCategoryJoins.Add(productCategoryJoin);
				}
			}
			
			Console.WriteLine("Inserting records to the ProductCategoryJoin table...");
			await _unitOfWork.Context.BulkInsertAsync(productCategoryJoins, bulkConfig);
			
			Console.WriteLine("Saving changes...");
            await _unitOfWork.Context.BulkSaveChangesAsync(bulkConfig);
            
			Console.WriteLine("Finished seeding products data.");
            return Ok("Data added successfully."); //  + output
        }
        
        
        [HttpGet]
		[Route("ratings/seed")]
		public async Task<IActionResult> SeedRatings() {
			List<RatingDto> ratings = RatingSeeder.Seed(null);
			
			BulkConfig bulkConfig = new BulkConfig() { BulkCopyTimeout = QUERY_TIMEOUT };
			await _unitOfWork.Context.BulkInsertAsync(_mapper.Map<List<Rating>>(ratings), bulkConfig);
			
			await _unitOfWork.Context.BulkSaveChangesAsync(bulkConfig);
			
			return Ok("Data added successfully."); // + output );
		}
        
        
        [HttpGet]
		[Route("reviews/seedClothings")]
		public async Task<IActionResult> SeedClothingReviews() {
			List<ReviewDto> reviews = ReviewSeeder.Seed(0);
			
			BulkConfig bulkConfig = new BulkConfig() { BulkCopyTimeout = QUERY_TIMEOUT };
			await _unitOfWork.Context.BulkInsertAsync(_mapper.Map<List<Review>>(reviews), bulkConfig);
			
			await _unitOfWork.Context.BulkSaveChangesAsync(bulkConfig);
			return Ok("Data added successfully."); //  + output 
		}
        
        
        [HttpGet]
		[Route("reviews/seedHome_Kitchen")]
		public async Task<IActionResult> SeedHome_KitchenReviews() {
			List<ReviewDto> reviews = ReviewSeeder.Seed(1);
			
			BulkConfig bulkConfig = new BulkConfig() { BulkCopyTimeout = QUERY_TIMEOUT};
			await _unitOfWork.Context.BulkInsertAsync(_mapper.Map<List<Review>>(reviews), bulkConfig);
			
			await _unitOfWork.Context.BulkSaveChangesAsync(bulkConfig);
			return Ok("Data added successfully."); // " + output );
		}
    }
}
*/