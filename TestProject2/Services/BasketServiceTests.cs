using AutoMapper;
using CourseTech.Core.DTOs.Basket;
using CourseTech.Core.Models;
using CourseTech.Core.Repositories;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Service.Services;
using Moq;
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

        [Fact]
        public async Task GetActiveBasketAsync_WhenBasketExists_ShouldReturnBasketDTO()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var basket = CreateBasket(userId);
            var dto = CreateBasketDTO(userId);

            _basketRepoMock.Setup(r => r.GetBasketByUserIdAsync(userId)).ReturnsAsync(basket);
            _mapperMock.Setup(m => m.Map<BasketDTO>(basket)).Returns(dto);

            // Act
            var result = await _basketService.GetActiveBasketAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(userId, result.Data.UserId);
        }

        [Fact]
        public async Task AddCourseToBasketAsync_WhenCourseMissing_ShouldReturnFailure()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _courseRepoMock.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync((Course)null!);

            var result = await _basketService.AddCourseToBasketAsync(userId, courseId);

            Assert.False(result.IsSuccess);
            Assert.Contains(CourseNotFoundMessage, result.ErrorMessage![0]);
        }

        // ...

        // ---------- Helpers ----------

        private static Course CreateSampleCourse(string title = "Test Course")
        {
            return new Course(
                title: title,
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

        private static Basket CreateBasket(Guid userId) => new Basket(userId);

        private static BasketDTO CreateBasketDTO(Guid userId, int itemCount = 0, string status = "Active")
            => new BasketDTO(Guid.NewGuid(), userId, new List<BasketItemDTO>(), status, itemCount);
    }
}