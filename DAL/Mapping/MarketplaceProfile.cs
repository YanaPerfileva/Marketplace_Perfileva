using AutoMapper;
using Marketplace.Data.Entities;
using Marketplace.Data.Dto;
using System.Linq;
using System.Collections.Generic;

namespace Marketplace.DAL.Mapping
{
    public class MarketplaceProfile : Profile
    {
        public MarketplaceProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt =>
                    opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.SellerName, opt =>
                    opt.MapFrom(src => src.Seller != null && src.Seller.User != null ? src.Seller.User.FullName : string.Empty))
                .ForMember(dest => dest.Skus, opt => opt.MapFrom(src => src.Skus ?? new List<ProductSku>()))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages ?? new List<ProductImage>()))
                .ForMember(dest => dest.Rating, opt =>
                    opt.MapFrom(src => src.Reviews != null && src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.ReviewCount, opt =>
                    opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0));

            CreateMap<ProductSku, SkuDto>();
            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<Review, ReviewDto>();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.SkuCode, o => o.MapFrom(s => s.ProductSku != null ? s.ProductSku.SkuCode : string.Empty))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ProductSku != null && s.ProductSku.Product != null ? s.ProductSku.Product.Name : string.Empty))
                .ForMember(d => d.Size, o => o.MapFrom(s => s.ProductSku != null ? s.ProductSku.Size : null))
                .ForMember(d => d.Color, o => o.MapFrom(s => s.ProductSku != null ? s.ProductSku.Color : null))
                .ForMember(d => d.PriceAtTime, o => o.MapFrom(s => s.PriceAtTime))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity))
                .ForMember(d => d.DiscountPercent, o => o.MapFrom(s => s.DiscountPercent));

            CreateMap<Order, OrderDto>()
                .ForMember(d => d.OrderNumber, o => o.MapFrom(s => s.OrderNumber ?? string.Empty))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Comment, o => o.MapFrom(s => s.Comment ?? string.Empty))
                .ForMember(d => d.SellerName, o => o.MapFrom(s => s.Seller != null && s.Seller.User != null ? s.Seller.User.FullName : string.Empty))
                .ForMember(d => d.ItemsCount, o => o.MapFrom(s => s.OrderItems != null ? s.OrderItems.Count : 0))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems != null ? s.OrderItems : new List<OrderItem>()));

            CreateMap<Payment, PaymentDto>();
            CreateMap<Shipping, ShippingDto>();

            CreateMap<Seller, SellerDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User != null ? src.User.Phone : string.Empty))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty));

            CreateMap<SellerStatistics, SellerStatisticsDto>();
            CreateMap<PopularProductView, PopularProductDto>();
            CreateMap<ActiveProductView, ActiveProductDto>();

            CreateMap<SellerProductDto, ProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand ?? string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName ?? string.Empty))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.ViewsCount, opt => opt.MapFrom(src => src.ViewsCount))
                .ForMember(dest => dest.PurchaseCount, opt => opt.MapFrom(src => src.PurchaseCount));
        }
    }
}
