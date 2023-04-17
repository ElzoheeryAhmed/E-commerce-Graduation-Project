using System.Linq.Expressions;
using GraduationProject.Models.Dto;
using X.PagedList;

namespace GraduationProject.IRepository
{
	public interface IGenericRepository<T> where T : class
	{
		/// <summary>
		/// Checks if an entity exists in the database that matches the given expression.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		Task<bool> existsAsync(Expression<Func<T, bool>> expression);
		
		/// <summary>
		/// Get the object of the specified type from the database if it matches the given expression.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="includes"></param>
		/// <returns></returns>
		Task<T> GetByAsync(Expression<Func<T, bool>> expression = null, List<string> includes = null);

		/// <summary>
		/// Get all objects of the specified type from the database, filter them with `expression` if not null, and order them by `orderBy` if not null.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="orderBy"></param>
		/// <param name="includes"></param>
		/// <returns></returns>
		Task<IList<T>> GetAllAsync(
			Expression<Func<T, bool>> expression = null,
			Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
			List<string> includes = null
		);
		
		/// <summary>
		/// Get one or more pages of objects of the specified type from the database, filter them with `expression` if not null, and order them by `orderBy` if not null.
		/// </summary>
		/// <param name="pagingFilter"></param>
		/// <param name="expression"></param>
		/// <param name="orderBy"></param>
		/// <param name="includes"></param>
		/// <returns></returns>
		Task<IPagedList<T>> GetPagedList(
			PagingFilter pagingFilter,
			Expression<Func<T, bool>> expression = null,
			Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
			List<string> includes = null
		);

		/// <summary>
		/// Insert the given entity into the Database.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task InsertAsync(T entity);

		/// <summary>
		/// Insert the given entities into the Database.
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		Task InsertRangeAsync(IEnumerable<T> entities);

		/// <summary>
		/// Deleting the entity that matches the given id from the Database .
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task<bool> DeleteAsync(Expression<Func<T, bool>> expression);

		/// <summary>
		/// /// Deleting the given entities from the Database.
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		void DeleteRangeAsync(IEnumerable<T> entities);

		/// <summary>
		/// Update the Database with given entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		void Update(T entity);
	}
}
