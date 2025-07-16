//using Xunit;
//using Moq;
//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using AutoMapper;
//using CourseTech.Core.Services;
//using CourseTech.Core.UnitOfWorks;
//using CourseTech.Core.Models;
//using CourseTech.Core.DTOs.AppUser;
//using CourseTech.Core.DTOs.Course;
//using CourseTech.Service.Services;

//namespace TestProject2.Services
//{
//    public class EnrollmentServiceTests
//    {
//        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
//        private readonly Mock<IMapper> _mapperMock = new();
//        private readonly EnrollmentService _enrollmentService;

//        public EnrollmentServiceTests()
//        {
//            _enrollmentService = new EnrollmentService(_unitOfWorkMock.Object, _mapperMock.Object);
//        }

//        [Fact]
//        public async Task EnrollAsync_ShouldFail_WhenAlreadyEnrolled()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var courseId = Guid.NewGuid();
//            var existingEnrollment = new Enrollment { UserId = userId, CourseId = courseId };

//            _unitOfWorkMock.Setup(x => x.Enrollment.GetEnrollmentAsync(userId, courseId))
//                .ReturnsAsync(existingEnrollment);

//            // Act
//            var result = await _enrollmentService.EnrollAsync(userId, courseId);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Contains("already enrolled", result.Message);
//        }

//        [Fact]
//        public async Task EnrollAsync_ShouldFail_WhenNoPurchasedCourse()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var courseId = Guid.NewGuid();

//            _unitOfWorkMock.Setup(x => x.Enrollment.GetEnrollmentAsync(userId, courseId))
//                .ReturnsAsync((Enrollment)null);
//            _unitOfWorkMock.Setup(x => x.Order.GetOrdersByUserIdAsync(userId))
//                .ReturnsAsync(new List<Order>());

//            // Act
//            var result = await _enrollmentService.EnrollAsync(userId, courseId);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Contains("no completed orders", result.Message);
//        }

//        [Fact]
//        public async Task EnrollAsync_ShouldFail_WhenCourseNotPurchased()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var courseId = Guid.NewGuid();
//            var orders = new List<Order>
//            {
//                new Order { Id = Guid.NewGuid(), UserId = userId, OrderItems = new List<OrderItem>() }
//            };

//            _unitOfWorkMock.Setup(x => x.Enrollment.GetEnrollmentAsync(userId, courseId))
//                .ReturnsAsync((Enrollment)null);
//            _unitOfWorkMock.Setup(x => x.Order.GetOrdersByUserIdAsync(userId))
//                .ReturnsAsync(orders);

//            // Act
//            var result = await _enrollmentService.EnrollAsync(userId, courseId);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Contains("not purchased", result.Message);
//        }

//        [Fact]
//        public async Task GetEnrolledCoursesByUserAsync_ShouldReturnEnrolledCourses()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var enrollments = new List<Enrollment>
//            {
//                new Enrollment { UserId = userId, Course = new Course { Id = Guid.NewGuid(), Title = "Course 1" } },
//                new Enrollment { UserId = userId, Course = new Course { Id = Guid.NewGuid(), Title = "Course 2" } }
//            };
//            var courseDtos = new List<CourseDTO>
//            {
//                new CourseDTO { Id = enrollments[0].Course.Id, Title = "Course 1" },
//                new CourseDTO { Id = enrollments[1].Course.Id, Title = "Course 2" }
//            };

//            _unitOfWorkMock.Setup(x => x.Enrollment.GetEnrollmentsByUserIdAsync(userId))
//                .ReturnsAsync(enrollments);
//            _mapperMock.Setup(x => x.Map<IEnumerable<CourseDTO>>(It.IsAny<IEnumerable<Course>>()))
//                .Returns(courseDtos);

//            // Act
//            var result = await _enrollmentService.GetEnrolledCoursesByUserAsync(userId);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(courseDtos, result.Data);
//        }

//        [Fact]
//        public async Task GetEnrolledUsersByCourseAsync_ShouldReturnEnrolledUsers()
//        {
//            // Arrange
//            var courseId = Guid.NewGuid();
//            var enrollments = new List<Enrollment>
//            {
//                new Enrollment { CourseId = courseId, AppUser = new AppUser { Id = Guid.NewGuid() } },
//                new Enrollment { CourseId = courseId, AppUser = new AppUser { Id = Guid.NewGuid() } }
//            };
//            var userDtos = new List<AppUserDTO>
//            {
//                new AppUserDTO { Id = enrollments[0].AppUser.Id },
//                new AppUserDTO { Id = enrollments[1].AppUser.Id }
//            };

//            _unitOfWorkMock.Setup(x => x.Enrollment.GetEnrollmentsByCourseIdAsync(courseId))
//                .ReturnsAsync(enrollments);
//            _mapperMock.Setup(x => x.Map<IEnumerable<AppUserDTO>>(It.IsAny<IEnumerable<AppUser>>()))
//                .Returns(userDtos);

//            // Act
//            var result = await _enrollmentService.GetEnrolledUsersByCourseAsync(courseId);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(userDtos, result.Data);
//        }

//        [Fact]
//        public async Task UnenrollAsync_ShouldRemoveEnrollment()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var courseId = Guid.NewGuid();
//            var enrollment = new Enrollment { UserId = userId, CourseId = courseId };

//            _unitOfWorkMock.Setup(x => x.Enrollment.GetEnrollmentAsync(userId, courseId))
//                .ReturnsAsync(enrollment);

//            // Act
//            var result = await _enrollmentService.UnenrollAsync(userId, courseId);

//            // Assert
//            Assert.True(result.IsSuccess);
//            _unitOfWorkMock.Verify(x => x.Enrollment.Delete(enrollment), Times.Once);
//            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
//        }
//    }
//}
