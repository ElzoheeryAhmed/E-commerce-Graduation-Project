
using System.ComponentModel;

namespace GraduationProject.Controllers.FilterParameters
{
    public interface IPagingFilter {
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }

    public class PagingFilter : IPagingFilter {
        const int maxPageSize = 50;
        private int _pageSize = 10;
        
        /// <summary>
        /// The page number of the paged list to return. Defaults to the first page (page 1).
        /// </summary>
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;
        
        /// <summary>
        /// The maximum number of items to return per page. Defaults to 10 and limited to 50.
        /// </summary>
        [DefaultValue(10)]
        public int PageSize {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
    
    public class Categories_BrandsPagingFilter : IPagingFilter {
        const int maxPageSize = 5000;
        private int _pageSize = 50;

        /// <summary>
        /// The page number of the paged list to return. Defaults to the first page (page 1).
        /// </summary>
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;
        
        /// <summary>
        /// The maximum number of items to return per page. Defaults to 50 and limited to 5000.
        /// </summary>
        [DefaultValue(50)]
        public int PageSize {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}
