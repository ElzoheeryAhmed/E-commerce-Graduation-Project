using GraduationProject.Data;
using GraduationProject.IRepository;
using GraduationProject.Models;

namespace GraduationProject.Repository
{
	public class UnitOfWork : IUnitOfWork {
		private readonly AppDbContext _context;
		private readonly IGenericRepository<Product> _products;
        private readonly IGenericRepository<ProductCategory> _productCategories;
		private readonly IGenericRepository<ProductCategoryJoin> _prodcutCategoryJoins;
        private readonly IGenericRepository<Brand> _brands;
        private readonly GenericRepository<ProductUpdate> _productUpdates;
		private readonly IGenericRepository<Rating> _ratings;
        private readonly GenericRepository<Review> _reviews;

        public UnitOfWork(AppDbContext context) {
			_context = context;
			_products = new GenericRepository<Product>(_context);
			_productCategories = new GenericRepository<ProductCategory>(_context);
			_prodcutCategoryJoins = new GenericRepository<ProductCategoryJoin>(_context);
			_brands = new GenericRepository<Brand>(_context);
			_productUpdates = new GenericRepository<ProductUpdate>(_context);
			_ratings = new GenericRepository<Rating>(_context);
			_reviews = new GenericRepository<Review>(_context);
		}

		// Encapsulation of _products and _ratings.
		public IGenericRepository<Product> Products => _products;
        public IGenericRepository<ProductCategory> ProductCategories => _productCategories;
        public IGenericRepository<Brand> Brands => _brands;
        public IGenericRepository<ProductUpdate> ProductUpdates => _productUpdates;
        public IGenericRepository<Rating> Ratings => _ratings;
        public IGenericRepository<Review> Reviews => _reviews;
        public IGenericRepository<ProductCategoryJoin> ProductCategoryJoins => _prodcutCategoryJoins;
        public AppDbContext Context => _context;


        public void Dispose() {
			// Garbage Collector: When the operations are done, free them from the memory.
			_context.Dispose();
			GC.SuppressFinalize(this);
		}

		public async Task Save() {
			await _context.SaveChangesAsync();
		}
	}
}
