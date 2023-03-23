using GraduationProject.Data;
using GraduationProject.IRepository;
using GraduationProject.Models;

namespace GraduationProject.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;
		private IGenericRepository<Product> _products;
		private IGenericRepository<Rating> _ratings;
        private GenericRepository<Review> _reviews;

        public UnitOfWork(AppDbContext context)
		{
			_context = context;
			_products = new GenericRepository<Product>(_context);
			_ratings = new GenericRepository<Rating>(_context);
			_reviews = new GenericRepository<Review>(_context);
		}

		// Encapsulation of _products and _ratings.
		public IGenericRepository<Product> Products => _products;
        public IGenericRepository<Rating> Ratings => _ratings;
        public IGenericRepository<Review> Reviews => _reviews;


        public void Dispose()
		{
			// Garbage Collector: When the operations are done, free them from the memory.
			_context.Dispose();
			GC.SuppressFinalize(this);
		}

		public async Task Save()
		{
			await _context.SaveChangesAsync();
		}
	}
}
