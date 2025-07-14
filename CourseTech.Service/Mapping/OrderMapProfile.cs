using AutoMapper;
using CourseTech.Core.DTOs.Order;
using CourseTech.Core.Models;

namespace CourseTech.Service.Mapping
{
    public class OrderMapProfile : Profile
    {
        public OrderMapProfile()
        {
            CreateMap<Order, OrderDTO>()
                .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
                .ForCtorParam("Email", opt => opt.MapFrom(src => src.AppUser.Email))
                .ForCtorParam("FullName", opt => opt.MapFrom(src => $"{src.AppUser.FirstName} {src.AppUser.LastName}"))
                .ForCtorParam("TotalPrice", opt => opt.MapFrom(src => src.TotalPrice))
                .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
                .ForCtorParam("OrderItems", opt => opt.MapFrom(src => src.OrderItems))
                .ForCtorParam("CreatedAt", opt => opt.MapFrom(src => src.CreatedAt)); 

            CreateMap<OrderItem, OrderItemDTO>()
                .ForCtorParam("CourseId", opt => opt.MapFrom(src => src.CourseId))
                .ForCtorParam("Title", opt => opt.MapFrom(src => src.Course.Title))
                .ForCtorParam("Price", opt => opt.MapFrom(src => src.Price))
                .ForCtorParam("ImageUrl", opt => opt.MapFrom(src => src.Course.ImageUrl));
        }
    }
}