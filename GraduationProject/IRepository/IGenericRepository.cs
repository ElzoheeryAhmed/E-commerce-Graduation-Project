using System.Linq.Expressions;

namespace GraduationProject.IRepository
{
	public interface IGenericRepository<T> where T : class
	{
		/// <summary>
		/// Get the object of the specified type from the database if it matches the given expression.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="includes"></param>
		/// <returns></returns>
		Task<T> GetByAsync(Expression<Func<T, bool>> expression = null, List<string> includes = null);

		/// <summary>
		/// Get all objects of the specified type from the database.
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
		Task<bool> DeleteAsync(int id);

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
		void UpdateAsync(T entity);
	}
}
