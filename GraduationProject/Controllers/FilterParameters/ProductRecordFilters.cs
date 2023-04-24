
namespace GraduationProject.Controllers.FilterParameters
{
    public class ProdcutRecordFilters
    {
        public string? ProductTitle { get; set; } = null;
        public decimal? ProductMinPrice { get; set; } = null;
        public decimal? ProductMaxPrice { get; set; } = null;
        public DateTime? ProductDateAdded { get; set; } = null;
        public double? ProductVoteAverage { get; set; } = null;
        public int? ProductBrandId { get; set; } = null;
        public List<int>? ProductCategoryIds { get; set; } = null;
    }
}
