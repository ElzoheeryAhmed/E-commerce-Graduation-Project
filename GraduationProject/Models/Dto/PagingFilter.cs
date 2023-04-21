
namespace GraduationProject.Models.Dto
{
    public interface IPagingFilter
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }

    public class PagingFilter : IPagingFilter {
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        
        public int PageSize {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
    
    public class Categories_BrandsPagingFilter : IPagingFilter {
        const int maxPageSize = 5000;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 50;
        
        public int PageSize {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}
