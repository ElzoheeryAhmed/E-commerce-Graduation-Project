using GraduationProject.Data;
using GraduationProject.IRepository;
using GraduationProject.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;
using X.PagedList;

namespace GraduationProject.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly AppDbContext _context;
		private readonly DbSet<T> _db;

		public GenericRepository(AppDbContext context) {
			_context = context;
			_db = _context.Set<T>();
		}
		
		
		public async Task<bool> existsAsync(Expression<Func<T, bool>> expression) {
			return await _db.AnyAsync(expression);
		}
		
		public async Task<T> GetByAsync(Expression<Func<T, bool>> expression = null, List<string> includes = null) {
			// Create an IQueryable<T> object from the _db property, which represents the database table for type T.
			IQueryable<T> query = _db;

			if (includes != null) {
				foreach (var includeProperty in includes) {
					// If the includes parameter is not null, load the related entities using the Include method.
					query = query.Include(includeProperty);
				}
			}

			// Execute the query and retrieve the first entity that matches the criteria using the FirstOrDefaultAsync method.
			// If the expression parameter is not null, filter the query by the given expression.
			// Calling AsNoTracking() before calling FirstOrDefaultAsync() can improve performance by reducing the amount of work that Entity Framework needs to do to track changes to the entity. Since GetByAsync is only retrieving an entity and not modifying it, there is no need to track changes to the entity, hence AsNoTracking() is used.
			return await query.AsNoTracking().FirstOrDefaultAsync(expression);
		}

		public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null) {
			IQueryable<T> query = _db;

			if (expression != null) {
				query = query.Where(expression);
			}

			if (includes != null) {
				foreach (var includeProperty in includes) {
					query = query.Include(includeProperty);
				}
			}

			if (orderBy != null) {
				query = orderBy(query);
			}

			return await query.AsNoTracking().ToListAsync();
		}

		public async Task<IPagedList<T>> GetPagedList(PagingFilter pagingfilter, Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null) {
            IQueryable<T> query = _db;

			if (expression != null) {
				query = query.Where(expression);
			}

			if (includes != null) {
				foreach (var includeProperty in includes) {
					query = query.Include(includeProperty);
				}
			}

			if (orderBy != null) {
				query = orderBy(query);
			}

			return await query.AsNoTracking().ToPagedListAsync(pagingfilter.PageNumber, pagingfilter.PageSize);
        }
		
		public async Task InsertAsync(T entity) {
			await _db.AddAsync(entity);
		}

		public async Task InsertRangeAsync(IEnumerable<T> entities) {
			await _db.AddRangeAsync(entities);
		}

		public async Task<bool> DeleteAsync(Expression<Func<T, bool>> expression) {
			var entity = await _db.AsNoTracking().FirstOrDefaultAsync(expression);

			if (entity == null) {
				Log.Error($"EntityNotFound: could don't find an entity that matches the given expression when executing 'DeleteAsync'");
				return false;
			}

			_db.Remove(entity);
			return true;
		}

		public void DeleteRangeAsync(IEnumerable<T> entities) {
			_db.RemoveRange(entities);
		}

		public void Update(T entity) {
			// This line attaches the entity to the _db context. Attaching an entity to a context allows the context to track changes to the entity and detect when it should be updated in the database.
			_db.Attach(entity);

			// This line sets the state of the entity to Modified in the _context. This tells the _context that the entity has been updated and needs to be saved to the database the next time changes are saved.
			_context.Entry(entity).State = EntityState.Modified;
		}
	}
}
