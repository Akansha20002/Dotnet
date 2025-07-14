using AutoMapper;
using CourseTech.Core.DTOs.Category;
using CourseTech.Core.Models;

namespace CourseTech.Service.Mapping
{
    public class CategoryMapProfile : Profile
    {
        public CategoryMapProfile()
        {
            CreateMap<Category, CategoryDTO>();

            CreateMap<CategoryCreateDTO, Category>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.Courses, opt => opt.Ignore());

            CreateMap<CategoryUpdateDTO, Category>()
                .ForMember(dest => dest.Courses, opt => opt.Ignore());
        }
    }
}