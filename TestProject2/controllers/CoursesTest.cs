using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.Course;
using CourseTech.Core.Services;
using CourseTech.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestProject2.controllers
{
    public class CoursesTest
    {
        private readonly Mock<ICourseService> _serviceMock;
        private readonly CoursesController _controller;

        public CoursesTest()
        {
            _serviceMock = new Mock<ICourseService>();
            _controller = new CoursesController(_serviceMock.Object);
        }

        private CourseDTO CreateSampleCourse() => new(
            Guid.NewGuid(), "Sample", "Desc", "image.jpg", "video.mp4",
            "Beginner", "English", 199.99m, TimeSpan.FromHours(3),
            DateTime.UtcNow, "Instructor", "Category", DateTime.UtcNow
        );

        private CourseSummaryDTO CreateSampleSummary() => new(
            Guid.NewGuid(), "Title", 149.99m, "image.jpg", "Instructor", "Intermediate"
        );

        [Fact]
        public async Task GetById_ReturnsOk_WhenCourseExists()
        {
            var id = Guid.NewGuid();
            var result = ServiceResult<CourseDTO>.Success(CreateSampleCourse());
            _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(result);

            var response = await _controller.GetById(id) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
            Assert.Equal(result, response.Value);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenNotFound()
        {
            var id = Guid.NewGuid();
            var result = ServiceResult<CourseDTO>.Fail("Not found");
            _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(result);

            var response = await _controller.GetById(id) as ObjectResult;

            Assert.Equal(400, response!.StatusCode);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithCourses()
        {
            var result = ServiceResult<IEnumerable<CourseDTO>>.Success(new[] { CreateSampleCourse() });
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(result);

            var response = await _controller.GetAll() as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
            Assert.Equal(result, response.Value);
        }

        [Fact]
        public async Task GetPublishedCourses_ReturnsOk()
        {
            var result = ServiceResult<IEnumerable<CourseDTO>>.Success(new[] { CreateSampleCourse() });
            _serviceMock.Setup(s => s.GetPublishedCoursesAsync()).ReturnsAsync(result);

            var response = await _controller.GetPublishedCourses() as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task GetCoursesByCategory_ReturnsOk()
        {
            var categoryId = Guid.NewGuid();
            var result = ServiceResult<IEnumerable<CourseSummaryDTO>>.Success(new[] { CreateSampleSummary() });

            _serviceMock.Setup(s => s.GetCoursesByCategoryAsync(categoryId)).ReturnsAsync(result);

            var response = await _controller.GetCoursesByCategory(categoryId) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task GetCoursesByInstructor_ReturnsOk()
        {
            var instructorId = Guid.NewGuid();
            var result = ServiceResult<IEnumerable<CourseListDTO>>.Success(new List<CourseListDTO>());

            _serviceMock.Setup(s => s.GetCoursesByInstructorAsync(instructorId)).ReturnsAsync(result);

            var response = await _controller.GetCoursesByInstructor(instructorId) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task GetCourseWithDetails_ReturnsOk()
        {
            var courseId = Guid.NewGuid();
            var result = ServiceResult<CourseDTO>.Success(CreateSampleCourse());

            _serviceMock.Setup(s => s.GetCourseWithDetailsAsync(courseId)).ReturnsAsync(result);

            var response = await _controller.GetCourseWithDetails(courseId) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task GetAllCoursesSummariesForCard_ReturnsOk()
        {
            var result = ServiceResult<IEnumerable<CourseSummaryDTO>>.Success(new[] { CreateSampleSummary() });

            _serviceMock.Setup(s => s.GetAllCoursesSummariesForCardAsync()).ReturnsAsync(result);

            var response = await _controller.GetAllCoursesSummariesForCard() as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenSuccessful()
        {
            var createDto = new CourseCreateDTO(
                "Course Title", "Description", "image.jpg", "video.mp4", "Beginner",
                "English", 299.99m, TimeSpan.FromHours(3), Guid.NewGuid(), Guid.NewGuid()
            );
            var result = ServiceResult<CourseDTO>.Success(CreateSampleCourse());

            _serviceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(result);

            var response = await _controller.Create(createDto) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccessful()
        {
            var updateDto = new CourseUpdateDTO(
                Guid.NewGuid(), "Updated Title", "Updated Description", "image.jpg", "video.mp4",
                "Advanced", "English", 399.99m, TimeSpan.FromHours(4), true, DateTime.UtcNow,
                Guid.NewGuid(), Guid.NewGuid()
            );
            var result = ServiceResult<CourseDTO>.Success(CreateSampleCourse());

            _serviceMock.Setup(s => s.UpdateAsync(updateDto)).ReturnsAsync(result);

            var response = await _controller.Update(updateDto) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task SoftDelete_ReturnsOk_WhenDeleted()
        {
            var courseId = Guid.NewGuid();
            var result = ServiceResult.Success();

            _serviceMock.Setup(s => s.SoftDeleteAsync(courseId)).ReturnsAsync(result);

            var response = await _controller.SoftDelete(courseId) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task PublishCourse_ReturnsOk()
        {
            var courseId = Guid.NewGuid();
            var result = ServiceResult.Success();

            _serviceMock.Setup(s => s.PublishCourseAsync(courseId)).ReturnsAsync(result);

            var response = await _controller.PublishCourse(courseId) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }

        [Fact]
        public async Task UnpublishCourse_ReturnsOk()
        {
            var courseId = Guid.NewGuid();
            var result = ServiceResult.Success();

            _serviceMock.Setup(s => s.UnpublishCourseAsync(courseId)).ReturnsAsync(result);

            var response = await _controller.UnpublishCourse(courseId) as ObjectResult;

            Assert.Equal(200, response!.StatusCode);
        }
    }
}
