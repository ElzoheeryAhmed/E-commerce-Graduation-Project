using GraduationProject.Controllers.FilterParameters;
using GraduationProject.Data;
using GraduationProject.Repository.Extensions;
using GraduationProject.IRepository;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;
using X.PagedList;

namespace GraduationProject.Repository
{
	public class GenericRepository<TSource> : IGenericRepository<TSource> where TSource : class {
		private readonly AppDbContext _context;
		private readonly DbSet<TSource> _db;

		public GenericRepository(AppDbContext context) {
			_context = context;
			_db = _context.Set<TSource>();
		}
		
		
		public async Task<bool> ExistsAsync(Expression<Func<TSource, bool>> expression) {
			return await _db.AsNoTracking().AnyAsync(expression);
		}
		
		public async Task<TSource>? GetByAsync(Expression<Func<TSource, bool>> expression, bool asNoTracking = false, List<string>? includeEntities = null, Expression<Func<TSource, TSource>>? selectExpression = null) {
			// Create an IQueryable<TSource> object from the _db property, which represents the database table for type T.
			IQueryable<TSource> query = _db;
			
			query = query.Where(expression);
			
			if (includeEntities != null) {
				foreach (var entity in includeEntities) {
					// If the includes parameter is not null, load the related entities using the Include method.
					query = query.Include(entity);
				}
			}
			
			if (selectExpression != null) {
				query = query.Select(selectExpression);
			}
			
			// Calling AsNoTracking() before calling FirstOrDefaultAsync() can improve performance by reducing the amount of work that Entity Framework needs to do to track changes to the entity. Since GetByAsync is only retrieving an entity and not modifying it, there is no need to track changes to the entity, hence AsNoTracking() is used.
			if (asNoTracking) {
				query = query.AsNoTracking();
			}
			
			else {
				query = query.AsTracking();
			}
			
			// Execute the query and retrieve the first entity that matches the criteria using the FirstOrDefaultAsync method.
			// If the expression parameter is not null, filter the query by the given expression.
			return await query.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<TSource>>? GetAllAsync(List<Expression<Func<TSource, bool>>>? expressions = null, IPagingFilter? pagingfilter=null, bool asNoTracking = false, List<string>? includeEntities = null, Expression<Func<TSource, TSource>>? selectExpression = null, string? orderBy = null) {
			IQueryable<TSource> query = _db;
			
			if (expressions != null) {
				// https://stackoverflow.com/questions/4098343/entity-framework-where-method-chaining
				foreach (var expression in expressions) {
					query = query.Where(expression);
				}
			}
			
			if (includeEntities != null) {
				foreach (var entity in includeEntities) {
					query = query.Include(entity);
				}
			}
			
			if (!string.IsNullOrWhiteSpace(orderBy)) {
				query = query.OrderBy(orderBy);
			}
			
			if (selectExpression != null) {
				query = query.Select(selectExpression);
			}
			
			// if (orderBy != null) {
			// 	query = orderBy(query);
			// }
			
			if (asNoTracking) {
				query = query.AsNoTracking();
			}
			
			else {
				query = query.AsTracking();
			}
			
			if (pagingfilter != null) {
				return await query.ToPagedListAsync(pagingfilter.PageNumber, pagingfilter.PageSize);
			}
			
			return await query.ToListAsync();
		}

		public async Task<TSource>? GetFieldByAsync(Expression<Func<TSource, bool>>? expression, Expression<Func<TSource, TSource>> fieldSelector, bool asNoTracking = false, string? includeEntity = null) {
			IQueryable<TSource> query = _db;
			
			if (expression != null) {
				query = query.Where(expression);
			}
			
			if (includeEntity != null) {
				query = query.Include(includeEntity);
			}
			
			if (asNoTracking) {
				query = query.AsNoTracking();
			}
			
			else {
				query = query.AsTracking();
			}
			
			// Select the field that matches the selection criteria using the Select method.
			return await query.Select(fieldSelector).FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<TSource>>? GetFieldFromAllAsync(Expression<Func<TSource, TSource>> fieldSelector, Expression<Func<TSource, bool>>? expression = null, IPagingFilter? pagingfilter=null, bool asNoTracking = false, string? includeEntity = null, string? orderBy = null) {
			IQueryable<TSource> query = _db;
			
			if (expression != null) {
				query = query.Where(expression);
			}
			
			if (includeEntity != null) {
				query = query.Include(includeEntity);
			}
			
			query = query.Select(fieldSelector);
			
			if (!string.IsNullOrWhiteSpace(orderBy)) {
				query = query.OrderBy(orderBy);
			}
			
			if (asNoTracking) {
				query = query.AsNoTracking();
			}
			
			else {
				query = query.AsTracking();
			}
			
			if (pagingfilter != null) {
				return await query.ToPagedListAsync(pagingfilter.PageNumber, pagingfilter.PageSize);
			}
			
			return await query.ToListAsync();
		}

		public async Task InsertAsync(TSource entity) {
			await _db.AddAsync(entity);
		}

		public async Task InsertRangeAsync(IEnumerable<TSource> entities) {
			await _db.AddRangeAsync(entities);
		}

		public async Task<bool> DeleteAsync(Expression<Func<TSource, bool>> expression) {
			var entity = await _db.AsNoTracking().FirstOrDefaultAsync(expression);

			if (entity == null) {
				Log.Error($"EntityNotFound: could don't find an entity that matches the given expression when executing 'DeleteAsync'");
				return false;
			}

			_db.Remove(entity);
			return true;
		}

		public void DeleteRangeAsync(IEnumerable<TSource> entities) {
			_db.RemoveRange(entities);
		}

		public void Update(TSource entity) {
			// This line attaches the entity to the _db context. Attaching an entity to a context allows the context to track changes to the entity and detect when it should be updated in the database.
			_db.Attach(entity);

			// This line sets the state of the entity to Modified in the _context. This tells the _context that the entity has been updated and needs to be saved to the database the next time changes are saved.
			_context.Entry(entity).State = EntityState.Modified;
		}
	}
}
