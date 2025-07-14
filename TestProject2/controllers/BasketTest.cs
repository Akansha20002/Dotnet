using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.Basket;
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
    public class BasketTest
    {
        private readonly Mock<IBasketService> _basketServiceMock;
        private readonly BasketsController _controller;

        private static readonly Guid TestUserId = Guid.NewGuid();
        private static readonly Guid TestCourseId = Guid.NewGuid();
        private static readonly Guid TestBasketId = Guid.NewGuid();

        public BasketTest()
        {
            _basketServiceMock = new Mock<IBasketService>(MockBehavior.Strict);
            _controller = new BasketsController(_basketServiceMock.Object);
        }

        private static BasketDTO CreateSampleBasket(Guid? userId = null) => new(
            TestBasketId,
            userId ?? TestUserId,
            new List<BasketItemDTO>(),
            "Active",
            199.99m
        );

        #region GetBasket

        [Fact]
        public async Task GetBasket_ValidUserId_ReturnsOkAsync()
        {
            var expected = ServiceResult<BasketDTO>.Success(CreateSampleBasket(TestUserId));
            _basketServiceMock.Setup(s => s.GetActiveBasketAsync(TestUserId)).ReturnsAsync(expected);

            var result = await _controller.GetBasket(TestUserId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
            _basketServiceMock.Verify(s => s.GetActiveBasketAsync(TestUserId), Times.Once);
        }

        [Fact]
        public async Task GetBasket_InvalidUserId_ReturnsBadRequestAsync()
        {
            var expected = ServiceResult<BasketDTO>.Fail("Basket not found");
            _basketServiceMock.Setup(s => s.GetActiveBasketAsync(TestUserId)).ReturnsAsync(expected);

            var result = await _controller.GetBasket(TestUserId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region AddCourseToBasket

        [Fact]
        public async Task AddCourseToBasket_ValidInput_ReturnsOkAsync()
        {
            _basketServiceMock.Setup(s => s.AddCourseToBasketAsync(TestUserId, TestCourseId)).ReturnsAsync(ServiceResult.Success());

            var result = await _controller.AddCourseToBasket(TestUserId, TestCourseId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            _basketServiceMock.Verify(s => s.AddCourseToBasketAsync(TestUserId, TestCourseId), Times.Once);
        }

        [Fact]
        public async Task AddCourseToBasket_CourseNotFound_ReturnsBadRequestAsync()
        {
            _basketServiceMock.Setup(s => s.AddCourseToBasketAsync(TestUserId, TestCourseId)).ReturnsAsync(ServiceResult.Fail("Course not found"));

            var result = await _controller.AddCourseToBasket(TestUserId, TestCourseId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        #endregion

        #region RemoveCourseFromBasket

        [Fact]
        public async Task RemoveCourseFromBasket_ValidInput_ReturnsOkAsync()
        {
            _basketServiceMock.Setup(s => s.RemoveCourseFromBasketAsync(TestUserId, TestCourseId)).ReturnsAsync(ServiceResult.Success());

            var result = await _controller.RemoveCourseFromBasket(TestUserId, TestCourseId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task RemoveCourseFromBasket_InvalidCourse_ReturnsBadRequestAsync()
        {
            _basketServiceMock.Setup(s => s.RemoveCourseFromBasketAsync(TestUserId, TestCourseId)).ReturnsAsync(ServiceResult.Fail("Course not found"));

            var result = await _controller.RemoveCourseFromBasket(TestUserId, TestCourseId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        #endregion

        #region ClearBasket

        [Fact]
        public async Task ClearBasket_Success_ReturnsOkAsync()
        {
            _basketServiceMock.Setup(s => s.ClearBasketAsync(TestUserId)).ReturnsAsync(ServiceResult.Success());

            var result = await _controller.ClearBasket(TestUserId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task ClearBasket_Failure_ReturnsBadRequestAsync()
        {
            _basketServiceMock.Setup(s => s.ClearBasketAsync(TestUserId)).ReturnsAsync(ServiceResult.Fail("Basket not found"));

            var result = await _controller.ClearBasket(TestUserId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        #endregion

        #region CompleteBasket

        [Fact]
        public async Task CompleteBasket_Success_ReturnsOkAsync()
        {
            _basketServiceMock.Setup(s => s.CompleteBasketAsync(TestUserId)).ReturnsAsync(ServiceResult.Success());

            var result = await _controller.CompleteBasket(TestUserId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task CompleteBasket_Failure_ReturnsBadRequestAsync()
        {
            _basketServiceMock.Setup(s => s.CompleteBasketAsync(TestUserId)).ReturnsAsync(ServiceResult.Fail("Basket cannot be completed"));

            var result = await _controller.CompleteBasket(TestUserId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        #endregion

        #region GetBasketWithItems

        [Fact]
        public async Task GetBasketWithItems_ValidId_ReturnsOkAsync()
        {
            _basketServiceMock.Setup(s => s.GetBasketWithItemsAsync(TestBasketId))
                .ReturnsAsync(ServiceResult<BasketDTO>.Success(CreateSampleBasket()));

            var result = await _controller.GetBasketWithItems(TestBasketId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetBasketWithItems_Failure_ReturnsBadRequestAsync()
        {
            _basketServiceMock.Setup(s => s.GetBasketWithItemsAsync(TestBasketId))
                .ReturnsAsync(ServiceResult<BasketDTO>.Fail("Basket not found"));

            var result = await _controller.GetBasketWithItems(TestBasketId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        #endregion
    }
}
