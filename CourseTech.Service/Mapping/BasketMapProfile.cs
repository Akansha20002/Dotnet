using AutoMapper;
using CourseTech.Core.DTOs.Basket;
using CourseTech.Core.Models;

namespace CourseTech.Service.Mapping
{
    public class BasketMapProfile : Profile
    {
        public BasketMapProfile()
        {
            CreateMap<Basket, BasketDTO>()
              .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
              .ForCtorParam("UserId", opt => opt.MapFrom(src => src.UserId))
              .ForCtorParam("Items", opt => opt.MapFrom(src => src.BasketItems))
              .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status))
              .ForCtorParam("TotalPrice", opt => opt.MapFrom(src => src.TotalPrice));
        }
    }
}