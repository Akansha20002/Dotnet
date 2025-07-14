using AutoMapper;
using CourseTech.Core.DTOs.AppUser;
using CourseTech.Core.Models;

namespace CourseTech.Service.Mapping
{
    public class AppUserMapProfile : Profile
    {
        public AppUserMapProfile()
        {
            CreateMap<AppUser, AppUserDTO>()
            .ConstructUsing(src => new AppUserDTO(
                src.Id,
                src.FirstName,
                src.LastName,
                src.Email!,
                src.PhoneNumber!
            )).ReverseMap();

            CreateMap<AppUser, AppUserWithPasswordDTO>()
            .ConstructUsing(src => new AppUserWithPasswordDTO(
                src.Id,
                src.FirstName,
                src.LastName,
                src.Email!,
                string.Empty,
                src.PhoneNumber!
            ));

            CreateMap<AppUser, AppUserWithNamesDTO>()
                .ConstructUsing(src => new AppUserWithNamesDTO(
                  src.Id,
                  $"{src.FirstName} {src.LastName}"
                  ));

            CreateMap<AppUserCreateDTO, AppUser>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<AppUserDTO, AppUser>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}