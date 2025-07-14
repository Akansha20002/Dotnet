using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.Basket;
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
    public class BasketsControllerTests
    {
        private Mock<IBasketService> _basketServiceMock;
        private BasketsController _controller;

        [SetUp]
        public void Setup()
        {
            _basketServiceMock = new Mock<IBasketService>();
            _controller = new BasketsController(_basketServiceMock.Object);
        }

        #region Get Basket

        [Test]
        public async Task GetBasket_ReturnsOk_WithBasket()
        {
            var userId = Guid.NewGuid();
            var basketDto = new BasketDTO(Guid.NewGuid(), userId, new List<BasketItemDTO>(), "Active", 199.99m);
            var serviceResult = ServiceResult<BasketDTO>.Success(basketDto);

            _basketServiceMock.Setup(x => x.GetActiveBasketAsync(userId)).ReturnsAsync(serviceResult);

            var result = await _controller.GetBasket(userId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        [Test]
        public async Task GetBasket_ReturnsBadRequest_WhenFailed()
        {
            var userId = Guid.NewGuid();
            var serviceResult = ServiceResult<BasketDTO>.Fail("Basket not found");

            _basketServiceMock.Setup(x => x.GetActiveBasketAsync(userId)).ReturnsAsync(serviceResult);

            var result = await _controller.GetBasket(userId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        #endregion

        #region Add Course to Basket

        [Test]
        public async Task AddCourseToBasket_ReturnsOk_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var serviceResult = ServiceResult.Success();

            _basketServiceMock.Setup(x => x.AddCourseToBasketAsync(userId, courseId)).ReturnsAsync(serviceResult);

            var result = await _controller.AddCourseToBasket(userId, courseId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        [Test]
        public async Task AddCourseToBasket_ReturnsBadRequest_WhenFailed()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var serviceResult = ServiceResult.Fail("Course not found");

            _basketServiceMock.Setup(x => x.AddCourseToBasketAsync(userId, courseId)).ReturnsAsync(serviceResult);

            var result = await _controller.AddCourseToBasket(userId, courseId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        #endregion

        #region Remove Course from Basket

        [Test]
        public async Task RemoveCourseFromBasket_ReturnsOk_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var serviceResult = ServiceResult.Success();

            _basketServiceMock.Setup(x => x.RemoveCourseFromBasketAsync(userId, courseId)).ReturnsAsync(serviceResult);

            var result = await _controller.RemoveCourseFromBasket(userId, courseId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        [Test]
        public async Task RemoveCourseFromBasket_ReturnsBadRequest_WhenFailed()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var serviceResult = ServiceResult.Fail("Course not found");

            _basketServiceMock.Setup(x => x.RemoveCourseFromBasketAsync(userId, courseId)).ReturnsAsync(serviceResult);

            var result = await _controller.RemoveCourseFromBasket(userId, courseId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        #endregion

        #region Clear Basket

        [Test]
        public async Task ClearBasket_ReturnsOk_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            var serviceResult = ServiceResult.Success();

            _basketServiceMock.Setup(x => x.ClearBasketAsync(userId)).ReturnsAsync(serviceResult);

            var result = await _controller.ClearBasket(userId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        [Test]
        public async Task ClearBasket_ReturnsBadRequest_WhenFailed()
        {
            var userId = Guid.NewGuid();
            var serviceResult = ServiceResult.Fail("Basket not found");

            _basketServiceMock.Setup(x => x.ClearBasketAsync(userId)).ReturnsAsync(serviceResult);

            var result = await _controller.ClearBasket(userId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        #endregion

        #region Complete Basket

        [Test]
        public async Task CompleteBasket_ReturnsOk_WhenSuccessful()
        {
            var userId = Guid.NewGuid();
            var serviceResult = ServiceResult.Success();

            _basketServiceMock.Setup(x => x.CompleteBasketAsync(userId)).ReturnsAsync(serviceResult);

            var result = await _controller.CompleteBasket(userId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        [Test]
        public async Task CompleteBasket_ReturnsBadRequest_WhenFailed()
        {
            var userId = Guid.NewGuid();
            var serviceResult = ServiceResult.Fail("Basket cannot be completed");

            _basketServiceMock.Setup(x => x.CompleteBasketAsync(userId)).ReturnsAsync(serviceResult);

            var result = await _controller.CompleteBasket(userId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        #endregion

        #region Get Basket With Items (Admin)

        [Test]
        public async Task GetBasketWithItems_ReturnsOk_WithBasket()
        {
            var basketId = Guid.NewGuid();
            var basketDto = new BasketDTO(basketId, Guid.NewGuid(), new List<BasketItemDTO>(), "Completed", 250.00m);
            var serviceResult = ServiceResult<BasketDTO>.Success(basketDto);

            _basketServiceMock.Setup(x => x.GetBasketWithItemsAsync(basketId)).ReturnsAsync(serviceResult);

            var result = await _controller.GetBasketWithItems(basketId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        [Test]
        public async Task GetBasketWithItems_ReturnsBadRequest_WhenFailed()
        {
            var basketId = Guid.NewGuid();
            var serviceResult = ServiceResult<BasketDTO>.Fail("Basket not found");

            _basketServiceMock.Setup(x => x.GetBasketWithItemsAsync(basketId)).ReturnsAsync(serviceResult);

            var result = await _controller.GetBasketWithItems(basketId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(serviceResult, objectResult.Value);
        }

        #endregion
    }
}
