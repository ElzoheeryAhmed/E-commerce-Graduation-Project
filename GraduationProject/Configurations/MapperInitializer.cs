using AutoMapper;
using GraduationProject.Models;
using GraduationProject.Models.Dto;

namespace GraduationProject.Configurations
{
	public class MapperInitializer : Profile {
		public MapperInitializer() {
			CreateMap<ProductCreateDto, Product>()
				.ForMember(dest => dest.ProductCategories, opt => opt.MapFrom(src => src.ProductCategories.Select(pc => new ProductCategoryJoin { ProductCategoryId = pc })))
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<Product, ProductDto>()
				.ForMember(dest => dest.ProductCategories, opts => {
					opts.Condition(src => (src.ProductCategories != null) && (src.ProductCategories.Count > 0) && (src.ProductCategories[0] != null));
					opts.MapFrom(src => src.ProductCategories.Select(pc => src.ProductCategories[0].ProductCategory != null ?
						new ProductCategoryDto { Id = pc.ProductCategoryId, Name = pc.ProductCategory.Name } : new ProductCategoryDto { Id = pc.ProductCategoryId } ));
				}).ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<ProductDto, Product>()
				.ForMember(dest => dest.ProductCategories, opt => opt.MapFrom(src => src.ProductCategories.Select(pc => new ProductCategoryJoin { ProductCategoryId = pc.Id })))
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<Product, ProductDtoWithBrand>()
				.ForMember(dest => dest.ProductCategories, opts => {
					opts.Condition(src => (src.ProductCategories != null) && (src.ProductCategories.Count > 0) && (src.ProductCategories[0] != null));
					opts.MapFrom(src => src.ProductCategories.Select(pc => src.ProductCategories[0].ProductCategory != null ?
						new ProductCategoryDto { Id = pc.ProductCategoryId, Name = pc.ProductCategory.Name } : new ProductCategoryDto { Id = pc.ProductCategoryId } ));
				}).ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<ProductCategory, ProductCategoryCreateDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<ProductCategory, ProductCategoryDto>()
				.ReverseMap()
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<Product, ProductUpdateDto>()
				.ForMember(dest => dest.ProductCategories, opt => opt.MapFrom(src => src.ProductCategories.Select(pc => pc.ProductCategoryId)))
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			CreateMap<ProductUpdateDto, Product>()
				.ForMember(dest => dest.ProductCategories, opts => {
					opts.Condition(src => (src.ProductCategories != null) && (src.ProductCategories.Count > 0));
					opts.MapFrom(src => src.ProductCategories.Select(pc => new ProductCategoryJoin { ProductCategoryId = pc }));})
				.ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
			
			
			CreateMap<Brand, BrandDto>()
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
