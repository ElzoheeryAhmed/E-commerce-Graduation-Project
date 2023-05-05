
namespace GraduationProject.Controllers.FilterParameters
{
    public class ProdcutRecordFilters
    {
        /// <summary>
        /// The product title to search for.
        /// </summary>
        public string? ProductTitle { get; set; } = null;
        /// <summary>
        /// The minimum price threshold. Defaults to zero.
        /// </summary>
        public decimal? ProductMinPrice { get; set; } = null;
        /// <summary>
        /// The maximum price threshold. By default, there is no limit.
        /// </summary>
        public decimal? ProductMaxPrice { get; set; } = null;
        /// <summary>
        /// A date to search for products added after it.
        /// </summary>
        public DateTime? ProductDateAdded { get; set; } = null;
        /// <summary>
        /// The minimum vote average threshold. Defaults to zero.
        /// </summary>
        public double? ProductVoteAverage { get; set; } = null;
        /// <summary>
        /// Filter products by brand id.
        /// </summary>
        public int? ProductBrandId { get; set; } = null;
        /// <summary>
        /// Filter products by category id(s).
        /// </summary>
        public List<int>? ProductCategoryIds { get; set; } = null;
    }
}
