using System.Linq.Expressions;
using GraduationProject.Controllers.FilterParameters;

namespace GraduationProject.IRepository
{
	public interface IGenericRepository<TSource> where TSource : class {
		/// <summary>
		/// Checks if an entity exists in the database that matches the given expression.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		Task<bool> ExistsAsync(Expression<Func<TSource, bool>> expression);
		
		/// <summary>
		/// Get the object of the specified type from the database if it matches the given expression.
		/// </summary>
		/// <param name="expression">Passed to the '.Where()' method.</param>
		/// <param name="asNoTracking">Specifies whether to disable or enable tracking.</param>
		/// <param name="includes">Specifies which nested entities to include.</param>
		/// <param name="selectExpression">Specifies which fields to return</param>
		/// <returns></returns>
		Task<TSource>? GetByAsync(
			Expression<Func<TSource, bool>> expression,
			bool asNoTracking = false,
			List<string>? includes = null,
			Expression<Func<TSource, TSource>>? selectExpression = null
		);

		/// <summary>
		/// Get one or more pages of objects if `PagingFilter` is not null otherwise all the objects of the specified type from the database.
		/// Filter them with `expression` if not null, and order them by `orderBy` if not null.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="pagingFilter"></param>
		/// <param name="asNoTracking"></param>
		/// <param name="includeEntities"></param>
		/// <param name="selectExpression"></param>
		/// <param name="orderBy"></param>
		/// <returns></returns>
		Task<IEnumerable<TSource>>? GetAllAsync(
			List<Expression<Func<TSource, bool>>>? expression = null,
			IPagingFilter? pagingFilter=null,
			bool asNoTracking = false,
			List<string>? includeEntities = null,
			Expression<Func<TSource, TSource>>? selectExpression = null,
			string? orderBy = null
			// Func<IQueryable<TSource>, IOrderedQueryable<TSource>>? orderBy = null
		);
		

		/// <summary>
		/// Get the field that matches the selection criteria.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="fieldSelector"></param>
		/// <param name="asNoTracking"></param>
		/// <param name="includeEntity"></param>
		/// <returns></returns>
		Task<TSource>? GetFieldByAsync(
			Expression<Func<TSource, bool>>? expression,
			Expression<Func<TSource, TSource>> fieldSelector,
			bool asNoTracking = false,
			string? includeEntity = null
		);

		/// <summary>
		/// Get the field that match the selection criteria.
		/// </summary>
		/// <param name="fieldSelector"></param>
		/// <param name="expression"></param>
		/// <param name="pagingFilter"></param>
		/// <param name="asNoTracking"></param>
		/// <param name="includeEntity"></param>
		/// <param name="orderBy"></param>
		/// <returns></returns>
		Task<IEnumerable<TSource>>? GetFieldFromAllAsync(
			Expression<Func<TSource, TSource>> fieldSelector,
			Expression<Func<TSource, bool>>? expression = null,
			IPagingFilter? pagingFilter=null,
			bool asNoTracking = false,
			string? includeEntity = null,
			string? orderBy = null
		);

		

		/// <summary>
		/// Insert the given entity into the Database.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task InsertAsync(TSource entity);

		/// <summary>
		/// Insert the given entities into the Database.
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		Task InsertRangeAsync(IEnumerable<TSource> entities);

		/// <summary>
		/// Deleting the entity that matches the given id from the Database .
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		Task<bool> DeleteAsync(Expression<Func<TSource, bool>> expression);

		/// <summary>
		/// /// Deleting the given entities from the Database.
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		void DeleteRangeAsync(IEnumerable<TSource> entities);

		/// <summary>
		/// Update the Database with given entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		void Update(TSource entity);
	}
}
