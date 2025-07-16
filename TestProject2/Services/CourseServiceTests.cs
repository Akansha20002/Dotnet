using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Core.Models;
using CourseTech.Core.DTOs.Course;
using CourseTech.Service.Services;
using Microsoft.AspNetCore.Identity;
using CourseTech.Shared.Enums;

namespace TestProject2.Xunit.Services
{
    public class CourseServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            _courseService = new CourseService(_unitOfWorkMock.Object, _userManagerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCourse_WhenExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course("Test Course", "Description", "image.jpg", "video.mp4", 
                CourseLevel.Beginner, CourseLanguage.English, 99.99m, TimeSpan.FromHours(2), 
                Guid.NewGuid(), Guid.NewGuid());

            var courseDto = new CourseDTO(courseId, "Test Course", "Description", "image.jpg", 
                "video.mp4", "Beginner", "English", 99.99m, TimeSpan.FromHours(2), 
                null, "Instructor Name", "Category Name", DateTime.UtcNow);

            _unitOfWorkMock.Setup(x => x.Course.GetByIdAsync(courseId))
                .ReturnsAsync(course);
            _mapperMock.Setup(x => x.Map<CourseDTO>(course))
                .Returns(courseDto);

            // Act
            var result = await _courseService.GetByIdAsync(courseId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(courseDto, result.Data);
        }

       
        [Fact]
        public async Task GetPublishedCoursesAsync_ShouldReturnOnlyPublishedCourses()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course("Course 1", "Description 1", "image1.jpg", "video1.mp4", 
                    CourseLevel.Beginner, CourseLanguage.English, 99.99m, TimeSpan.FromHours(2), 
                    Guid.NewGuid(), Guid.NewGuid()),
                new Course("Course 2", "Description 2", "image2.jpg", "video2.mp4", 
                    CourseLevel.Intermediate, CourseLanguage.Turkish, 149.99m, TimeSpan.FromHours(3), 
                    Guid.NewGuid(), Guid.NewGuid())
            };

            foreach (var course in courses)
            {
                course.Publish();
            }

            var courseDtos = new List<CourseDTO>
            {
                new CourseDTO(Guid.NewGuid(), "Course 1", "Description 1", "image1.jpg", "video1.mp4", 
                    "Beginner", "English", 99.99m, TimeSpan.FromHours(2), DateTime.UtcNow, "Instructor 1", "Category 1", DateTime.UtcNow),
                new CourseDTO(Guid.NewGuid(), "Course 2", "Description 2", "image2.jpg", "video2.mp4", 
                    "Intermediate", "Turkish", 149.99m, TimeSpan.FromHours(3), DateTime.UtcNow, "Instructor 2", "Category 2", DateTime.UtcNow)
            };

            _unitOfWorkMock.Setup(x => x.Course.GetPublishedCoursesAsync())
                .ReturnsAsync(courses);
            _mapperMock.Setup(x => x.Map<IEnumerable<CourseDTO>>(courses))
                .Returns(courseDtos);

            // Act
            var result = await _courseService.GetPublishedCoursesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(courseDtos, result.Data);
        }

        
        [Fact]
        public async Task PublishCourseAsync_ShouldPublishCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course("Test Course", "Description", "image.jpg", "video.mp4", 
                CourseLevel.Beginner, CourseLanguage.English, 99.99m, TimeSpan.FromHours(2), 
                Guid.NewGuid(), Guid.NewGuid());

            _unitOfWorkMock.Setup(x => x.Course.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _courseService.PublishCourseAsync(courseId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(course.IsPublished);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UnpublishCourseAsync_ShouldUnpublishCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course("Test Course", "Description", "image.jpg", "video.mp4", 
                CourseLevel.Beginner, CourseLanguage.English, 99.99m, TimeSpan.FromHours(2), 
                Guid.NewGuid(), Guid.NewGuid());
            course.Publish(); // Make it published first

            _unitOfWorkMock.Setup(x => x.Course.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _courseService.UnpublishCourseAsync(courseId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(course.IsPublished);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
