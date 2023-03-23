using GraduationProject.Models;

namespace GraduationProject.IRepository
{
	// the IDisposable interface is typically used to ensure that any unmanaged resources used by the unit of work, such as database connections, are properly released when the unit of work is no longer needed. This can help to prevent resource leaks and improve the performance and reliability of the application.
	// By implementing the IDisposable interface, the unit of work can expose a Dispose() method that can be used to release any unmanaged resources. It can be called manually or automatically when the unit of work is used in a using block.
	public interface IUnitOfWork : IDisposable
	{
		IGenericRepository<Product> Products { get; }
		IGenericRepository<Rating> Ratings { get; }
		IGenericRepository<Review> Reviews { get; }
		Task Save();
	}
}
