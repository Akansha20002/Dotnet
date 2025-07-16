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

namespace TestProject2.Xunit.Services
{
    public class BasketServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IBasketRepository> _basketRepoMock = new();
        private readonly Mock<ICourseRepository> _courseRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly BasketService _basketService;

        private const string CourseNotFoundMessage = "Course not found";
        private const string BasketNotFoundMessage = "Basket not found";

        public BasketServiceTests()
        {
            _unitOfWorkMock.Setup(u => u.Basket).Returns(_basketRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Course).Returns(_courseRepoMock.Object);
            _basketService = new BasketService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact, Trait("Method", "GetActiveBasketAsync")]
        public async Task GetActiveBasketAsync_WhenBasketExists_ShouldReturnBasketDTO()
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

        [Fact, Trait("Method", "GetActiveBasketAsync")]
        public async Task GetActiveBasketAsync_WhenBasketNotFound_ShouldCreateNewBasket()
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

        [Fact, Trait("Method", "AddCourseToBasketAsync")]
        public async Task AddCourseToBasketAsync_WhenCourseMissing_ShouldReturnFailure()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _courseRepoMock.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync((Course)null!);

            var result = await _basketService.AddCourseToBasketAsync(userId, courseId);

            Assert.False(result.IsSuccess);
            Assert.Contains(CourseNotFoundMessage, result.ErrorMessage![0]);
        }

        [Fact, Trait("Method", "AddCourseToBasketAsync")]
        public async Task AddCourseToBasketAsync_WhenBasketExists_ShouldAddCourse()
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

        [Fact, Trait("Method", "AddCourseToBasketAsync")]
        public async Task AddCourseToBasketAsync_WhenBasketNotFound_ShouldCreateNewBasketAndAddCourse()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var course = CreateSampleCourse();

            _courseRepoMock.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(course);
            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync((Basket)null!);

            var result = await _basketService.AddCourseToBasketAsync(userId, courseId);

            Assert.True(result.IsSuccess);
            _basketRepoMock.Verify(r => r.InsertAsync(It.IsAny<Basket>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        }

        [Fact, Trait("Method", "ClearBasketAsync")]
        public async Task ClearBasketAsync_WhenCalled_ShouldClearItems()
        {
            var userId = Guid.NewGuid();
            var course = CreateSampleCourse();
            var basket = new Basket(userId);
            basket.AddCourse(course);

            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync(basket);

            var result = await _basketService.ClearBasketAsync(userId);

            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact, Trait("Method", "CompleteBasketAsync")]
        public async Task CompleteBasketAsync_WhenItemsExist_ShouldSetCompletedStatus()
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

        [Fact, Trait("Method", "RemoveCourseFromBasketAsync")]
        public async Task RemoveCourseFromBasketAsync_WhenItemExists_ShouldRemoveCourse()
        {
            var userId = Guid.NewGuid();
            var course = CreateSampleCourse();
            var basket = new Basket(userId);
            basket.AddCourse(course);

            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync(basket);

            var result = await _basketService.RemoveCourseFromBasketAsync(userId, course.Id);

            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact, Trait("Method", "GetBasketWithItemsAsync")]
        public async Task GetBasketWithItemsAsync_WhenNotFound_ShouldReturnFailure()
        {
            var basketId = Guid.NewGuid();

            _basketRepoMock.Setup(r => r.GetBasketWithItemsAsync(basketId)).ReturnsAsync((Basket)null!);

            var result = await _basketService.GetBasketWithItemsAsync(basketId);

            Assert.False(result.IsSuccess);
            Assert.Contains(BasketNotFoundMessage, result.ErrorMessage![0]);
        }

        [Fact, Trait("Method", "GetBasketWithItemsAsync")]
        public async Task GetBasketWithItemsAsync_WhenFound_ShouldReturnMappedDTO()
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

        private static Course CreateSampleCourse()
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
