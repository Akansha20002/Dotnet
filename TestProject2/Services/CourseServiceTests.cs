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
            var course = CreateTestCourse();
            var courseDto = CreateTestCourseDto(courseId);

            _unitOfWorkMock.Setup(x => x.Course.GetByIdAsync(courseId))
                .ReturnsAsync(course);
            SetupMapperForCourseDTO(course, courseDto);

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
                CreateTestCourse("Course 1", "Description 1", CourseLevel.Beginner, CourseLanguage.English),
                CreateTestCourse("Course 2", "Description 2", CourseLevel.Intermediate, CourseLanguage.Turkish)
            };

            foreach (var course in courses)
                course.Publish();

            var courseDtos = new List<CourseDTO>
            {
                CreateTestCourseDto(Guid.NewGuid(), "Course 1", "Description 1", "Beginner", "English", "Instructor 1", "Category 1"),
                CreateTestCourseDto(Guid.NewGuid(), "Course 2", "Description 2", "Intermediate", "Turkish", "Instructor 2", "Category 2")
            };

            _unitOfWorkMock.Setup(x => x.Course.GetPublishedCoursesAsync())
                .ReturnsAsync(courses);
            SetupMapperForCourseDTOList(courses, courseDtos);

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
            var course = CreateTestCourse();

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
            var course = CreateTestCourse();
            course.Publish(); // initially published

            _unitOfWorkMock.Setup(x => x.Course.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _courseService.UnpublishCourseAsync(courseId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(course.IsPublished);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        // -------------------- Helper Methods --------------------

        private Course CreateTestCourse(string title = "Test Course", string description = "Description",
            CourseLevel level = CourseLevel.Beginner, CourseLanguage language = CourseLanguage.English)
        {
            return new Course(
                title,
                description,
                "image.jpg",
                "video.mp4",
                level,
                language,
                99.99m,
                TimeSpan.FromHours(2),
                Guid.NewGuid(),
                Guid.NewGuid()
            );
        }

        private CourseDTO CreateTestCourseDto(Guid id, string title = "Test Course", string description = "Description",
            string level = "Beginner", string language = "English",
            string instructorName = "Instructor Name", string categoryName = "Category Name")
        {
            return new CourseDTO(
                id,
                title,
                description,
                "image.jpg",
                "video.mp4",
                level,
                language,
                99.99m,
                TimeSpan.FromHours(2),
                DateTime.UtcNow,
                instructorName,
                categoryName,
                DateTime.UtcNow
            );
        }

        private void SetupMapperForCourseDTO(Course course, CourseDTO dto)
        {
            _mapperMock.Setup(x => x.Map<CourseDTO>(course)).Returns(dto);
        }

        private void SetupMapperForCourseDTOList(IEnumerable<Course> courses, IEnumerable<CourseDTO> dtos)
        {
            _mapperMock.Setup(x => x.Map<IEnumerable<CourseDTO>>(courses)).Returns(dtos);
        }
    }
}
