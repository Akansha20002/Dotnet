using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.AppUser;
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
    public class EnrollmentTests
    {
        private readonly Mock<IEnrollmentService> _serviceMock;
        private readonly EnrollmentsController _controller;
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _courseId = Guid.NewGuid();

        public EnrollmentTests()
        {
            _serviceMock = new Mock<IEnrollmentService>(MockBehavior.Strict);
            _controller = new EnrollmentsController(_serviceMock.Object);
        }

        #region Enroll

        [Fact]
        [Trait("Enrollment", "Enroll")]
        public async Task Enroll_WithValidData_ReturnsOk()
        {
            var result = ServiceResult.Success();
            _serviceMock.Setup(s => s.EnrollAsync(_userId, _courseId)).ReturnsAsync(result);

            var response = await _controller.Enroll(_userId, _courseId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(200, obj.StatusCode);
            Assert.Equal(result, obj.Value);
            _serviceMock.Verify(s => s.EnrollAsync(_userId, _courseId), Times.Once);
        }

        [Fact]
        [Trait("Enrollment", "Enroll")]
        public async Task Enroll_WhenAlreadyEnrolled_ReturnsBadRequest()
        {
            var result = ServiceResult.Fail("User already enrolled");
            _serviceMock.Setup(s => s.EnrollAsync(_userId, _courseId)).ReturnsAsync(result);

            var response = await _controller.Enroll(_userId, _courseId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(result, obj.Value);
        }

        [Fact]
        [Trait("Enrollment", "Enroll")]
        public async Task Enroll_WithEmptyUserId_ReturnsBadRequest()
        {
            var result = ServiceResult.Fail("Invalid user");
            _serviceMock.Setup(s => s.EnrollAsync(Guid.Empty, _courseId)).ReturnsAsync(result);

            var response = await _controller.Enroll(Guid.Empty, _courseId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(result, obj.Value);
        }

        [Fact]
        [Trait("Enrollment", "Enroll")]
        public async Task Enroll_WhenExceptionThrown_ReturnsException()
        {
            _serviceMock.Setup(s => s.EnrollAsync(_userId, _courseId)).ThrowsAsync(new Exception("Unexpected"));

            await Assert.ThrowsAsync<Exception>(() => _controller.Enroll(_userId, _courseId));
        }

        [Fact]
        [Trait("Enrollment", "Enroll")]
        public async Task Enroll_Twice_ReturnsSameFailure()
        {
            var failResult = ServiceResult.Fail("User already enrolled");

            _serviceMock.Setup(s => s.EnrollAsync(_userId, _courseId)).ReturnsAsync(failResult);

            var res1 = await _controller.Enroll(_userId, _courseId);
            var res2 = await _controller.Enroll(_userId, _courseId);

            Assert.Equal(failResult, (res1 as ObjectResult)?.Value);
            Assert.Equal(failResult, (res2 as ObjectResult)?.Value);
        }

        #endregion

        #region Unenroll

        [Fact]
        [Trait("Enrollment", "Unenroll")]
        public async Task Unenroll_WithValidInput_ReturnsOk()
        {
            _serviceMock.Setup(s => s.UnenrollAsync(_userId, _courseId)).ReturnsAsync(ServiceResult.Success());

            var response = await _controller.Unenroll(_userId, _courseId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(200, obj.StatusCode);
        }

        [Fact]
        [Trait("Enrollment", "Unenroll")]
        public async Task Unenroll_WhenNotFound_ReturnsBadRequest()
        {
            var fail = ServiceResult.Fail("Enrollment not found");
            _serviceMock.Setup(s => s.UnenrollAsync(_userId, _courseId)).ReturnsAsync(fail);

            var response = await _controller.Unenroll(_userId, _courseId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(fail, obj.Value);
        }

        [Fact]
        [Trait("Enrollment", "Unenroll-Reenroll")]
        public async Task Unenroll_Then_Reenroll_ReturnsOk()
        {
            _serviceMock.SetupSequence(s => s.UnenrollAsync(_userId, _courseId))
                        .ReturnsAsync(ServiceResult.Success());
            _serviceMock.SetupSequence(s => s.EnrollAsync(_userId, _courseId))
                        .ReturnsAsync(ServiceResult.Success());

            var res1 = await _controller.Unenroll(_userId, _courseId);
            var res2 = await _controller.Enroll(_userId, _courseId);

            Assert.Equal(200, (res1 as ObjectResult)?.StatusCode);
            Assert.Equal(200, (res2 as ObjectResult)?.StatusCode);
        }

        #endregion

        #region GetEnrolledCoursesByUser

        [Fact]
        [Trait("Enrollment", "GetEnrolledCoursesByUser")]
        public async Task GetEnrolledCoursesByUser_WithCourses_ReturnsOk()
        {
            var result = ServiceResult<IEnumerable<CourseDTO>>.Success(new[]
            {
                new CourseDTO(Guid.NewGuid(), "Test", "Desc", "Beginner", "Cat", "img.jpg", "vid.mp4", 199.99m, TimeSpan.FromHours(2), DateTime.UtcNow, "Instructor", "English", DateTime.UtcNow)
            });

            _serviceMock.Setup(s => s.GetEnrolledCoursesByUserAsync(_userId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledCoursesByUser(_userId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(200, obj.StatusCode);
            Assert.Equal(result, obj.Value);
        }

        [Fact]
        [Trait("Enrollment", "GetEnrolledCoursesByUser")]
        public async Task GetEnrolledCoursesByUser_WhenNotFound_ReturnsBadRequest()
        {
            var result = ServiceResult<IEnumerable<CourseDTO>>.Fail("No enrolled courses");
            _serviceMock.Setup(s => s.GetEnrolledCoursesByUserAsync(_userId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledCoursesByUser(_userId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(result, obj.Value);
        }

        #endregion

        #region GetEnrolledUsersByCourse

        [Fact]
        [Trait("Enrollment", "GetEnrolledUsersByCourse")]
        public async Task GetEnrolledUsersByCourse_WithUsers_ReturnsOk()
        {
            var users = new List<AppUserDTO> { new(Guid.NewGuid(), "user@example.com", "John", "Doe", null) };
            var result = ServiceResult<IEnumerable<AppUserDTO>>.Success(users);

            _serviceMock.Setup(s => s.GetEnrolledUsersByCourseAsync(_courseId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledUsersByCourse(_courseId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(200, obj.StatusCode);
            Assert.Equal(result, obj.Value);
        }

        [Fact]
        [Trait("Enrollment", "GetEnrolledUsersByCourse")]
        public async Task GetEnrolledUsersByCourse_WhenNotFound_ReturnsBadRequest()
        {
            var result = ServiceResult<IEnumerable<AppUserDTO>>.Fail("No enrolled users");

            _serviceMock.Setup(s => s.GetEnrolledUsersByCourseAsync(_courseId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledUsersByCourse(_courseId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(result, obj.Value);
        }

        [Fact]
        [Trait("Enrollment", "GetEnrolledUsersByCourse")]
        public async Task GetEnrolledUsersByCourse_WithEmptyList_ReturnsOk()
        {
            var result = ServiceResult<IEnumerable<AppUserDTO>>.Success(new List<AppUserDTO>());
            _serviceMock.Setup(s => s.GetEnrolledUsersByCourseAsync(_courseId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledUsersByCourse(_courseId);

            var obj = Assert.IsType<ObjectResult>(response);
            Assert.Equal(200, obj.StatusCode);
            Assert.Equal(result, obj.Value);
        }

        #endregion
    }
}
