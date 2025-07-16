using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Core.Models;
using CourseTech.Core.DTOs.Basket;
using CourseTech.Service.Services;
using CourseTech.Core.Repositories;
using CourseTech.Shared;

namespace TestProject2.Services
{
    public class BasketServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IBasketRepository> _basketRepoMock = new();
        private readonly Mock<ICourseRepository> _courseRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly BasketService _basketService;

        public BasketServiceTests()
        {
            _unitOfWorkMock.Setup(u => u.Basket).Returns(_basketRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Course).Returns(_courseRepoMock.Object);
            _basketService = new BasketService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetActiveBasketAsync_ShouldReturnExistingBasket()
        {
            var userId = Guid.NewGuid();
            var basket = new Basket(userId);
            var basketDto = new BasketDTO(Guid.NewGuid(), userId, new List<BasketItemDTO>(), "Active", 0);

            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync(basket);
            _mapperMock.Setup(m => m.Map<BasketDTO>(basket)).Returns(basketDto);

            var result = await _basketService.GetActiveBasketAsync(userId);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetActiveBasketAsync_ShouldCreateBasket_IfNotExists()
        {
            var userId = Guid.NewGuid();
            var basketDto = new BasketDTO(Guid.NewGuid(), userId, new List<BasketItemDTO>(), "Active", 0);

            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync((Basket)null!);
            _mapperMock.Setup(m => m.Map<BasketDTO>(It.IsAny<Basket>())).Returns(basketDto);

            var result = await _basketService.GetActiveBasketAsync(userId);

            Assert.True(result.IsSuccess);
            _basketRepoMock.Verify(r => r.InsertAsync(It.IsAny<Basket>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddCourseToBasketAsync_ShouldFail_IfCourseNotFound()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _courseRepoMock.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync((Course)null!);

            var result = await _basketService.AddCourseToBasketAsync(userId, courseId);

            Assert.False(result.IsSuccess);
            Assert.Contains("Course not found", result.ErrorMessage![0]);
        }

        [Fact]
        public async Task AddCourseToBasketAsync_ShouldSucceed_IfBasketExists()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var basket = new Basket(userId);
            var course = CreateSampleCourse();

            _courseRepoMock.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(course);
            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync(basket);

            var result = await _basketService.AddCourseToBasketAsync(userId, courseId);

            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddCourseToBasketAsync_ShouldCreateBasket_IfNotExists()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var course = CreateSampleCourse();

            _courseRepoMock.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(course);
            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync((Basket)null!);

            var result = await _basketService.AddCourseToBasketAsync(userId, courseId);

            Assert.True(result.IsSuccess);
            _basketRepoMock.Verify(r => r.InsertAsync(It.IsAny<Basket>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2)); // one for creating basket, one for adding course
        }

        [Fact]
        public async Task ClearBasketAsync_ShouldCallClearMethods()
        {
            var userId = Guid.NewGuid();
            var course = CreateSampleCourse();

            var basket = new Basket(userId);
            basket.AddCourse(course); // adds item so basket is not empty

            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync(basket);

            var result = await _basketService.ClearBasketAsync(userId);

            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CompleteBasketAsync_ShouldSetCompletedState()
        {
            var userId = Guid.NewGuid();
            var course = CreateSampleCourse();
            var basket = new Basket(userId);
            basket.AddCourse(course);

            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync(basket);

            var result = await _basketService.CompleteBasketAsync(userId);

            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveCourseFromBasketAsync_ShouldRemoveCourse()
        {
            var userId = Guid.NewGuid();
            var course = CreateSampleCourse(); // courseId is embedded inside

            var basket = new Basket(userId);
            basket.AddCourse(course); // Add it to the basket first

            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync(basket);

            var result = await _basketService.RemoveCourseFromBasketAsync(userId, course.Id); // Use correct ID

            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task GetBasketWithItemsAsync_ShouldReturnFail_IfNotFound()
        {
            var basketId = Guid.NewGuid();

            _basketRepoMock.Setup(r => r.GetBasketWithItemsAsync(basketId)).ReturnsAsync((Basket)null!);

            var result = await _basketService.GetBasketWithItemsAsync(basketId);

            Assert.False(result.IsSuccess);
            Assert.Contains("Basket not found", result.ErrorMessage![0]);
        }

        [Fact]
        public async Task GetBasketWithItemsAsync_ShouldReturnMappedDTO()
        {
            var basketId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var basket = new Basket(userId);
            var dto = new BasketDTO(Guid.NewGuid(), userId, new List<BasketItemDTO>(), "Active", 0);

            _basketRepoMock.Setup(r => r.GetBasketWithItemsAsync(basketId)).ReturnsAsync(basket);
            _mapperMock.Setup(m => m.Map<BasketDTO>(basket)).Returns(dto);

            var result = await _basketService.GetBasketWithItemsAsync(basketId);

            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Data);
        }

        private Course CreateSampleCourse()
        {
            return new Course(
                title: "Test Course",
                description: "Description",
                imageUrl: "http://image.com",
                videoUrl: "http://video.com",
                level: CourseTech.Shared.Enums.CourseLevel.Beginner,
                language: CourseTech.Shared.Enums.CourseLanguage.English,
                price: 100,
                duration: TimeSpan.FromHours(5),
                instructorId: Guid.NewGuid(),
                categoryId: Guid.NewGuid()
            );
        }
    }
}
