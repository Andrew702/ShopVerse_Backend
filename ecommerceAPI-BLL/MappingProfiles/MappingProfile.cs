using AutoMapper;
using ecommerceAPI.BLL.DTOs.Request;
using ecommerceAPI.BLL.DTOs.Response;
using ecommerceAPI.DAL.Entities;

namespace ecommerceAPI.BLL.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product → ProductResponse
        CreateMap<Product, ProductResponse>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand.Name))
            .ForMember(d => d.AverageRating, o => o.MapFrom(s =>
                s.Reviews.Any() ? Math.Round(s.Reviews.Average(r => (double)r.Rating), 1) : 0.0))
            .ForMember(d => d.ReviewCount, o => o.MapFrom(s => s.Reviews.Count));

        // Product → ProductDetailResponse
        CreateMap<Product, ProductDetailResponse>();

        // Category → CategoryResponse
        CreateMap<Category, CategoryResponse>();

        // Brand → BrandResponse
        CreateMap<Brand, BrandResponse>();

        // Review → ReviewResponse
        CreateMap<Review, ReviewResponse>();

        // CartItem → CartItemResponse
        CreateMap<CartItem, CartItemResponse>()
            .ForMember(d => d.ProductTitle, o => o.MapFrom(s => s.Product.Title))
            .ForMember(d => d.ProductImage, o => o.MapFrom(s => s.Product.Image))
            .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.Product.Price));

        // Order → OrderResponse
        CreateMap<Order, OrderResponse>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // OrderItem → OrderItemResponse
        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(d => d.ProductTitle, o => o.MapFrom(s => s.Product.Title))
            .ForMember(d => d.ProductImage, o => o.MapFrom(s => s.Product.Image));

        // Wishlist → WishlistResponse
        CreateMap<Wishlist, WishlistResponse>()
            .ForMember(d => d.ProductTitle, o => o.MapFrom(s => s.Product.Title))
            .ForMember(d => d.ProductPrice, o => o.MapFrom(s => s.Product.Price))
            .ForMember(d => d.ProductImage, o => o.MapFrom(s => s.Product.Image));

        // RegisterRequest → User
        CreateMap<RegisterRequest, User>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.Phone));
    }
}
