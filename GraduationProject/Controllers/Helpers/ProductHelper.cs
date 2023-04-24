
using System.Linq.Expressions;
using GraduationProject.Controllers.FilterParameters;
using GraduationProject.Models;
using GraduationProject.Models.Interfaces;

namespace GraduationProject.Controllers.Helpers
{
    public class ProductHelper<TSource> where TSource : IProductInfo {
        /// <summary>
        /// This method is used to get the name of the entities to include in the query based on the includedFields parameter.
        /// </summary>
        /// <param name="fieldsFilters"></param>
        /// <returns></returns>
        public static List<string> GetNameOfEntitiesToInclude(ProductFieldsFilter fieldsFilters) {
            var entitiesToInclude = new List<string> { "ProductCategories.ProductCategory", "Brand", "Reviews", "Ratings" };
            
            if (!string.IsNullOrWhiteSpace(fieldsFilters.OnlySelectFields)) {
                var includeFieldsList = fieldsFilters.OnlySelectFields.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                entitiesToInclude.RemoveAll(x => !includeFieldsList.Contains(x.Split('.', StringSplitOptions.RemoveEmptyEntries)[0].Trim()));
            }
            
            if (!string.IsNullOrWhiteSpace(fieldsFilters.FieldsToExclude)) {
                var fieldsToExcludeList = fieldsFilters.FieldsToExclude.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                entitiesToInclude.RemoveAll(x => fieldsToExcludeList.Contains(x.Split('.', StringSplitOptions.RemoveEmptyEntries)[0].Trim()));
            }
            
            return entitiesToInclude;
        }
        
        /// <summary>
        /// Zero values are not ignored when mapping. This method is used to keep the data of the existing product if the data of the productDto is zero.
        /// </summary>
        /// <param name="productDto">Represents the new data.</param>
        /// <param name="product">Represents the original data.</param>
        /// <returns></returns>
        public static void KeepOriginalIfNewIsZero(TSource productDto, Product product) {
            if (productDto.Price == 0)
                productDto.Price = product.Price;
            if (productDto.Discount == 0)
                productDto.Discount = product.Discount;
            if (productDto.BrandId == 0)
                productDto.BrandId = product.BrandId;
            if (productDto.Quantity == 0)
                productDto.Quantity = product.Quantity;
        }
    }

    public class ProductHelperWithVotes<T> : ProductHelper<T> where T : IProductInfoWithVotes
    {   
        /// <summary>
        /// Zero values are not ignored when mapping. This method is used to keep the data of the existing product if the data of the productDto is zero.
        /// </summary>
        /// <param name="productDto">Represents the new data.</param>
        /// <param name="product">Represents the original data.</param>
        /// <returns></returns>
        public static new void KeepOriginalIfNewIsZero(T productDto, Product product) {
            ProductHelper<T>.KeepOriginalIfNewIsZero(productDto, product);
            if (productDto.VoteCount == 0)
                productDto.VoteCount = product.VoteCount;
            
            if (productDto.VoteAverage == 0)
                productDto.VoteAverage = product.VoteAverage;
        }
    }
    
    public class ProductHelper {
        
        /// <summary>
        /// This method is used to get the filters for the products based on the recordFilters parameter.
        /// </summary>
        /// <param name="recordFilters"></param>
        /// <returns></returns>
        public static List<Expression<Func<Product, bool>>> GetProductFilters(ProdcutRecordFilters recordFilters) {
            List<Expression<Func<Product, bool>>> filterExpressions = new List<Expression<Func<Product, bool>>>();
            
            if (recordFilters != null) {
					if (!string.IsNullOrWhiteSpace(recordFilters.ProductTitle))
						filterExpressions.Add(p => p.Title.Contains(recordFilters.ProductTitle));
					
					if (recordFilters.ProductMinPrice != null && recordFilters.ProductMaxPrice != null)
						filterExpressions.Add(p => p.Price >= recordFilters.ProductMinPrice && p.Price <= recordFilters.ProductMaxPrice);
					
                    else if (recordFilters.ProductMinPrice != null)
						filterExpressions.Add(p => p.Price >= recordFilters.ProductMinPrice);
					
                    else if (recordFilters.ProductMaxPrice != null)
						filterExpressions.Add(p => p.Price <= recordFilters.ProductMaxPrice);
					
					if (recordFilters.ProductDateAdded != null)
						filterExpressions.Add(p => p.DateAdded >= recordFilters.ProductDateAdded);
					
					if (recordFilters.ProductVoteAverage != null)
						filterExpressions.Add(p => p.VoteAverage >= recordFilters.ProductVoteAverage);
					
					if (recordFilters.ProductBrandId != null)
						filterExpressions.Add(p => p.Brand.Id == recordFilters.ProductBrandId);
					
					if (recordFilters.ProductCategoryIds != null && recordFilters.ProductCategoryIds.Count > 0) {
                        // Matches only proudcts with the exact same categories as `recordFilters.ProductCategoryIds`.
                        // filterExpressions.Add(p => p.ProductCategories.Select(pcj => pcj.ProductCategoryId).All(pcId => recordFilters.ProductCategoryIds.Contains(pcId)));
                        
                        // Matches only products that have categories from the `recordFilters.ProductCategoryIds` list.
                        foreach (var categoryId in recordFilters.ProductCategoryIds) {
                            filterExpressions.Add(p => p.ProductCategories.Any(pcj => pcj.ProductCategoryId == categoryId));
                        }
                    }
				}
            
            return filterExpressions;
        }
    }
}
