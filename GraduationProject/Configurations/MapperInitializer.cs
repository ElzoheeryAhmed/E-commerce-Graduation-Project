using AutoMapper;
using GraduationProject.Models;
using GraduationProject.Models.Dto;

namespace GraduationProject.Configurations
{
	public class MapperInitializer : Profile
	{
		public MapperInitializer()
		{
			CreateMap<Product, ProductCreateDto>().ReverseMap();
			CreateMap<Product, ProductDto>().ReverseMap();
			
			CreateMap<User, UserLoginDto>().ReverseMap();
			CreateMap<User, UserCreateDto>().ReverseMap();
			
			CreateMap<Rating, RatingCreateDto>().ReverseMap();
			CreateMap<Rating, RatingDto>().ReverseMap();
			
			CreateMap<Review, ReviewCreateDto>().ReverseMap();
			CreateMap<Review, ReviewDto>().ReverseMap();
			
		}
	}
}
