
using GraduationProject.Models;
using GraduationProject.Models.Interfaces;

namespace GraduationProject.Controllers.Helpers
{
    public class ProductHelper<T> where T : IProductInfo {
        /// <summary>
        /// This method is used to get the name of the entities to include in the query based on the includedFields parameter.
        /// </summary>
        /// <param name="includeFields"></param>
        /// <returns></returns>
        public static List<string> GetNameOfEntitiesToInclude(string? includeFields) {
            List<string> entitiesToInclude = new List<string>();
            if (string.IsNullOrWhiteSpace(includeFields)) {
                entitiesToInclude = new List<string>() { "ProductCategories.ProductCategory", "Brand", "Reviews", "Ratings" };
            }
            
            else {
                List<string> includeFieldsList = includeFields.Split(',').ToList();
                if (includeFieldsList.Contains("ProductCategories")) {
                    entitiesToInclude.Add("ProductCategories.ProductCategory");
                }
                
                if (includeFieldsList.Contains("Brand")) {
                    entitiesToInclude.Add("Brand");
                }
                
                if (includeFieldsList.Contains("Reviews")) {
                    entitiesToInclude.Add("Reviews");
                }
                
                if (includeFieldsList.Contains("Ratings")) {
                    entitiesToInclude.Add("Ratings");
                }
            }
            
            return entitiesToInclude;
        }
        
        /// <summary>
        /// Zero values are not ignored when mapping. This method is used to keep the data of the existing product if the data of the productDto is zero.
        /// </summary>
        /// <param name="productDto">Represents the new data.</param>
        /// <param name="product">Represents the original data.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void KeepOriginalIfNewIsZero(T productDto, Product product) {
            if (productDto.Price == 0)
                productDto.Price = product.Price;
            if (productDto.Discount == 0)
                productDto.Discount = product.Discount;
            if (productDto.BrandId == 0)
                productDto.BrandId = product.BrandId;
        }
    }
    
    public class ProductHelperWithVotes<T> : ProductHelper<T> where T : IProductInfoWithVotes
    {
        // /// <summary>
        // /// This method is used to get the name of the entities to include in the query based on the includedFields parameter.
        // /// </summary>
        // /// <param name="includedFields"></param>
        // /// <returns></returns>
        // public static new List<string> GetNameOfIntitiesToInclude(string? includedFields) {
        //     List<string> entitiesToInclude = ProductHelper<T>.GetNameOfIntitiesToInclude(includedFields);
            
        //     return entitiesToInclude;
        // }
        
        /// <summary>
        /// Zero values are not ignored when mapping. This method is used to keep the data of the existing product if the data of the productDto is zero.
        /// </summary>
        /// <param name="productDto">Represents the new data.</param>
        /// <param name="product">Represents the original data.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static new void KeepOriginalIfNewIsZero(T productDto, Product product) {
            ProductHelper<T>.KeepOriginalIfNewIsZero(productDto, product);
            if (productDto.VoteCount == 0)
                productDto.VoteCount = product.VoteCount;
            
            if (productDto.VoteAverage == 0)
                productDto.VoteAverage = product.VoteAverage;
        }
    }
}