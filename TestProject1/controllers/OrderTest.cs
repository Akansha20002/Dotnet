using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.Order;
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
    public class OrderTest
    {
        private Mock<IOrderService> _orderServiceMock;
        private OrdersController _controller;

        [SetUp]
        public void Setup()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _controller = new OrdersController(_orderServiceMock.Object);
        }

        private OrderDTO CreateSampleOrder()
        {
            return new OrderDTO(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "test@example.com",
                "John Doe",
                299.99m,
                "Completed",
                new List<OrderItemDTO>(),
                DateTime.UtcNow
            );
        }

        #region CreateOrderFromBasket

        [Test]
        public async Task CreateOrderFromBasket_ReturnsOk_WhenSuccessful()
        {
            var basketId = Guid.NewGuid();
            var orderDto = CreateSampleOrder();
            var result = ServiceResult<OrderDTO>.Success(orderDto);

            _orderServiceMock
                .Setup(x => x.CreateOrderFromBasketAsync(basketId))
                .ReturnsAsync(result);

            var response = await _controller.CreateOrderFromBasket(basketId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task CreateOrderFromBasket_ReturnsBadRequest_WhenFailed()
        {
            var basketId = Guid.NewGuid();
            var result = ServiceResult<OrderDTO>.Fail("Basket not found");

            _orderServiceMock
                .Setup(x => x.CreateOrderFromBasketAsync(basketId))
                .ReturnsAsync(result);

            var response = await _controller.CreateOrderFromBasket(basketId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task CreateOrderFromBasket_ReturnsBadRequest_WhenBasketIdIsEmpty()
        {
            var basketId = Guid.Empty;
            var result = ServiceResult<OrderDTO>.Fail("Invalid basket ID");

            _orderServiceMock
                .Setup(x => x.CreateOrderFromBasketAsync(basketId))
                .ReturnsAsync(result);

            var response = await _controller.CreateOrderFromBasket(basketId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        #endregion

        #region GetOrderById

        [Test]
        public async Task GetOrderById_ReturnsOk_WhenFound()
        {
            var orderId = Guid.NewGuid();
            var orderDto = CreateSampleOrder();
            var result = ServiceResult<OrderDTO>.Success(orderDto);

            _orderServiceMock
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(result);

            var response = await _controller.GetOrderById(orderId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task GetOrderById_ReturnsBadRequest_WhenNotFound()
        {
            var orderId = Guid.NewGuid();
            var result = ServiceResult<OrderDTO>.Fail("Order not found");

            _orderServiceMock
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(result);

            var response = await _controller.GetOrderById(orderId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task GetOrderById_ReturnsBadRequest_WhenServiceReturnsNull()
        {
            var orderId = Guid.NewGuid();

            _orderServiceMock
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync((ServiceResult<OrderDTO>)null!);

            var response = await _controller.GetOrderById(orderId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
        }

        [Test]
        public void GetOrderById_ThrowsException_ReturnsException()
        {
            var orderId = Guid.NewGuid();

            _orderServiceMock
                .Setup(x => x.GetOrderByIdAsync(orderId))
                .ThrowsAsync(new Exception("Unexpected error"));

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _controller.GetOrderById(orderId);
            });
        }

        #endregion

        #region GetOrdersByUserId

        [Test]
        public async Task GetOrdersByUserId_ReturnsOk_WithOrders()
        {
            var userId = Guid.NewGuid();
            var orders = new List<OrderDTO>
            {
                CreateSampleOrder(),
                CreateSampleOrder()
            };
            var result = ServiceResult<List<OrderDTO>>.Success(orders);

            _orderServiceMock
                .Setup(x => x.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(result);

            var response = await _controller.GetOrdersByUserId(userId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task GetOrdersByUserId_ReturnsBadRequest_WhenNoOrders()
        {
            var userId = Guid.NewGuid();
            var result = ServiceResult<List<OrderDTO>>.Fail("No orders found");

            _orderServiceMock
                .Setup(x => x.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(result);

            var response = await _controller.GetOrdersByUserId(userId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task GetOrdersByUserId_ReturnsBadRequest_WhenUserIdIsEmpty()
        {
            var userId = Guid.Empty;
            var result = ServiceResult<List<OrderDTO>>.Fail("Invalid user ID");

            _orderServiceMock
                .Setup(x => x.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(result);

            var response = await _controller.GetOrdersByUserId(userId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        #endregion
    }
}
