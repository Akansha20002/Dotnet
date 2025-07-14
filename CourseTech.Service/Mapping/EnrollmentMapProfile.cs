using AutoMapper;
using CourseTech.Core.DTOs.Course;
using CourseTech.Core.Models;

namespace CourseTech.Service.Mapping
{
    public class EnrollmentMapProfile : Profile
    {
        public EnrollmentMapProfile()
        {
            CreateMap<Enrollment, CourseDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Course.Id))
               .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Course.Title))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Course.Description))
               .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Course.ImageUrl))
               .ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.Course.VideoUrl))
               .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Course.Level))
               .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Course.Language))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Course.Price))
               .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Course.Duration))
               .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.Course.PublishedAt))
               .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => $"{src.Course.Instructor.FirstName + src.Course.Instructor.LastName}"))
               .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Course.Title));
        }
    }
}
