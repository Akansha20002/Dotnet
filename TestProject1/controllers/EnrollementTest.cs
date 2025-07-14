using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.AppUser;
using CourseTech.Core.DTOs.Course;
using CourseTech.Core.Services;
using CourseTech.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProject1.Controllers
{
    [TestFixture]
    public class EnrollmentTest
    {
        private Mock<IEnrollmentService> _serviceMock;
        private EnrollmentsController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IEnrollmentService>();
            _controller = new EnrollmentsController(_serviceMock.Object);
        }

        #region Enroll

        [Test]
        public async Task Enroll_ReturnsOk_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var result = ServiceResult.Success();

            _serviceMock.Setup(s => s.EnrollAsync(userId, courseId)).ReturnsAsync(result);

            var response = await _controller.Enroll(userId, courseId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task Enroll_ReturnsBadRequest_WhenFailed()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var result = ServiceResult.Fail("User already enrolled");

            _serviceMock.Setup(s => s.EnrollAsync(userId, courseId)).ReturnsAsync(result);

            var response = await _controller.Enroll(userId, courseId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }
        [Test]
        public async Task Enroll_ReturnsBadRequest_WhenUserIdIsEmpty()
        {
            var userId = Guid.Empty;
            var courseId = Guid.NewGuid();

            var result = ServiceResult.Fail("Invalid user");

            _serviceMock.Setup(s => s.EnrollAsync(userId, courseId)).ReturnsAsync(result);

            var response = await _controller.Enroll(userId, courseId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }
        [Test]
        public async Task Enroll_ThrowsException_Returns500()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _serviceMock.Setup(s => s.EnrollAsync(userId, courseId)).ThrowsAsync(new Exception("Something went wrong"));

            Assert.ThrowsAsync<Exception>(() => _controller.Enroll(userId, courseId));
        }

        [Test]
        public async Task Enroll_Twice_ReturnsSameFailure()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var failResult = ServiceResult.Fail("User already enrolled");

            _serviceMock.Setup(s => s.EnrollAsync(userId, courseId)).ReturnsAsync(failResult);

            var response1 = await _controller.Enroll(userId, courseId);
            var response2 = await _controller.Enroll(userId, courseId);

            Assert.AreEqual((response1 as ObjectResult)?.Value, failResult);
            Assert.AreEqual((response2 as ObjectResult)?.Value, failResult);
        }


        #endregion

        #region Unenroll

        [Test]
        public async Task Unenroll_ReturnsOk_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var result = ServiceResult.Success();

            _serviceMock.Setup(s => s.UnenrollAsync(userId, courseId)).ReturnsAsync(result);

            var response = await _controller.Unenroll(userId, courseId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task Unenroll_ReturnsBadRequest_WhenFailed()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var result = ServiceResult.Fail("Enrollment not found");

            _serviceMock.Setup(s => s.UnenrollAsync(userId, courseId)).ReturnsAsync(result);

            var response = await _controller.Unenroll(userId, courseId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }
        [Test]
        public async Task Unenroll_Then_Reenroll_ReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _serviceMock.SetupSequence(s => s.UnenrollAsync(userId, courseId))
                .ReturnsAsync(ServiceResult.Success());

            _serviceMock.SetupSequence(s => s.EnrollAsync(userId, courseId))
                .ReturnsAsync(ServiceResult.Success());

            var unenrollResponse = await _controller.Unenroll(userId, courseId);
            var enrollResponse = await _controller.Enroll(userId, courseId);

            Assert.AreEqual(200, (unenrollResponse as ObjectResult)!.StatusCode);
            Assert.AreEqual(200, (enrollResponse as ObjectResult)!.StatusCode);
        }


        #endregion

        #region GetEnrolledCoursesByUser

        [Test]
        public async Task GetEnrolledCoursesByUser_ReturnsOk_WithCourses()
        {
            var userId = Guid.NewGuid();
            var courses = new List<CourseDTO>
            {
                new CourseDTO(
                    Guid.NewGuid(), "Test Course", "Description", "Beginner", "Category",
                    "image.jpg", "video.mp4", 199.99m, TimeSpan.FromHours(2),
                    DateTime.UtcNow, "Instructor", "English", DateTime.UtcNow)
            };

            var result = ServiceResult<IEnumerable<CourseDTO>>.Success(courses);

            _serviceMock.Setup(s => s.GetEnrolledCoursesByUserAsync(userId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledCoursesByUser(userId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task GetEnrolledCoursesByUser_ReturnsBadRequest_WhenNotFound()
        {
            var userId = Guid.NewGuid();
            var result = ServiceResult<IEnumerable<CourseDTO>>.Fail("No enrolled courses");

            _serviceMock.Setup(s => s.GetEnrolledCoursesByUserAsync(userId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledCoursesByUser(userId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        #endregion

        #region GetEnrolledUsersByCourse

        [Test]
        public async Task GetEnrolledUsersByCourse_ReturnsOk_WithUsers()
        {
            var courseId = Guid.NewGuid();
            var users = new List<AppUserDTO>
            {
                new AppUserDTO(Guid.NewGuid(), "user@example.com", "John", "Doe", null)
            };

            var result = ServiceResult<IEnumerable<AppUserDTO>>.Success(users);

            _serviceMock.Setup(s => s.GetEnrolledUsersByCourseAsync(courseId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledUsersByCourse(courseId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task GetEnrolledUsersByCourse_ReturnsBadRequest_WhenNotFound()
        {
            var courseId = Guid.NewGuid();
            var result = ServiceResult<IEnumerable<AppUserDTO>>.Fail("No enrolled users");

            _serviceMock.Setup(s => s.GetEnrolledUsersByCourseAsync(courseId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledUsersByCourse(courseId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }
        [Test]
        public async Task GetEnrolledUsersByCourse_ReturnsOk_WithEmptyList()
        {
            var courseId = Guid.NewGuid();
            var result = ServiceResult<IEnumerable<AppUserDTO>>.Success(new List<AppUserDTO>());

            _serviceMock.Setup(s => s.GetEnrolledUsersByCourseAsync(courseId)).ReturnsAsync(result);

            var response = await _controller.GetEnrolledUsersByCourse(courseId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        #endregion

    }
}
