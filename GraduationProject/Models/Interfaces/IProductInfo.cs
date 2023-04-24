
namespace GraduationProject.Models.Interfaces
{
    public interface IProductInfo {
        decimal Price { get; set; }
        decimal Discount { get; set; }
        int BrandId { get; set; }
        int Quantity { get; set; }
    }
    
    public interface IProductInfoWithVotes : IProductInfo {
        double VoteAverage { get; set; }
        int VoteCount { get; set; }
    }
}
