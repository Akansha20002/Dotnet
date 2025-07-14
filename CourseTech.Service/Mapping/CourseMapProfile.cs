using AutoMapper;
using CourseTech.Core.DTOs.Course;
using CourseTech.Core.Models;

namespace CourseTech.Service.Mapping
{
    public class CourseMapProfile : Profile
    {
        public CourseMapProfile()
        {
            CreateMap<CourseDTO, Course>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore());

            CreateMap<Course, CourseDTO>()
              .ConstructUsing(src => new CourseDTO(
                  src.Id,
                  src.Title,
                  src.Description,
                  src.ImageUrl,
                  src.VideoUrl,
                  src.Level.ToString(),
                  src.Language.ToString(),
                  src.Price,
                  src.Duration,
                  src.PublishedAt,
                  $"{src.Instructor.FirstName} {src.Instructor.LastName}",
                  src.Category.Name,
                  src.CreatedAt
              ));


            CreateMap<Course, CourseSummaryDTO>()
                .ConstructUsing(src => new CourseSummaryDTO(
                    src.Id,
                    src.Title,
                    src.Price,
                    src.ImageUrl,
                    $"{src.Instructor.FirstName} {src.Instructor.LastName}",
                    src.Level.ToString()
                ));   
            
            CreateMap<Course, CourseListDTO>()
                .ConstructUsing(src => new CourseListDTO(
                    src.Id,
                    src.Title,
                    src.Description,
                    src.Price,
                    src.ImageUrl,
                    src.Category.Name,
                    $"{src.Instructor.FirstName} {src.Instructor.LastName}",
                    src.Duration,
                    src.Level.ToString(),
                    src.IsPublished
                ));

            CreateMap<CourseCreateDTO, Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InstructorId, opt => opt.MapFrom(src => src.InstructorId))
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<CourseUpdateDTO, Course>()
                .ForMember(dest => dest.IsPublished, opt => opt.Ignore())
                .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore());
        }
    }
}