using System.Linq.Expressions;
using GraduationProject.Controllers.FilterParameters;

namespace GraduationProject.Repository.Extensions
{
    public class QueryableExtensions<TSource> where TSource : class {
        /// <summary>
        /// Generates an expression that selects specific fields from an entity object based on a given filter.
        /// </summary>
        /// <param name="fieldsFilters">The filters to apply when selecting fields.</param>
        /// <param name="entityType">The type of the entity.</param>
        /// <returns>An expression that selects the specified fields from an entity object.</returns>
        /// <remarks>
        /// This method can be used to generate an expression that selects a subset of fields from an entity object,
        /// based on the given filter. The filter can specify a list of fields to include or exclude. By default, all fields are included.
        /// The returned expression can be used as a projection in a LINQ query to avoid loading unnecessary data from the database.
        /// </remarks>
        public static Expression<Func<TSource, TSource>> EntityFieldsSelector(EntityFieldsFilter fieldsFilters, Type? entityType = null) {
            if (entityType == null)
                entityType = typeof(TSource);
            
            List<string> entityFields = new List<string>(entityType.GetProperties().Select(e => e.Name));
            
            if (!string.IsNullOrWhiteSpace(fieldsFilters.OnlySelectFields)) {
                var includeFieldsList = fieldsFilters.OnlySelectFields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                entityFields.RemoveAll(x => !includeFieldsList.Contains(x));
            }
            
            if (!string.IsNullOrWhiteSpace(fieldsFilters.FieldsToExclude)) {
                entityFields.RemoveAll(x => fieldsFilters.FieldsToExclude.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Contains(x));
            }
            
            var entity = Expression.Parameter(typeof(TSource), "entity");
            var entityDto = Expression.Parameter(typeof(TSource), "entityDto");
            var entityDtoInit = Expression.MemberInit(Expression.New(typeof(TSource)), entityFields.Select(x => Expression.Bind(typeof(TSource).GetProperty(x), Expression.Property(entity, x))));
            
            return Expression.Lambda<Func<TSource, TSource>>(entityDtoInit, entity);
        }
    }
}
