using AutoMapper;
using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Models.Authentication;

namespace CourseTech.Service.Mapping
{
    public class RefreshTokenMapProfile : Profile
    {
        public RefreshTokenMapProfile()
        {
            CreateMap<AppUserRefreshTokenDTO, AppUserRefreshToken>().ReverseMap();

        }
    }
}