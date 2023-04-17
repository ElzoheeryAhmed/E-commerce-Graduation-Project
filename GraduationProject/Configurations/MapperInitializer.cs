using AutoMapper;
using GraduationProject.Models;
using GraduationProject.Models.Dto;

namespace GraduationProject.Configurations
{
	public class MapperInitializer : Profile
	{
		public MapperInitializer()
		{
			CreateMap<Product, ProductCreateDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<Product, ProductDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<ProductCategory, ProductCategoryCreateDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<ProductCategory, ProductCategoryDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<Product, ProductUpdateDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			
			CreateMap<User, UserLoginDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<User, UserCreateDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			
			CreateMap<Rating, RatingCreateDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<Rating, RatingDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<Rating, RatingUpdateDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			
			CreateMap<Review, ReviewCreateDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<Review, ReviewDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<Review, ReviewUpdateDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
		}
	}
}
