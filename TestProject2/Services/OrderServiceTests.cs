//using Xunit;
//using Moq;
//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using AutoMapper;
//using CourseTech.Core.Services;
//using CourseTech.Core.UnitOfWorks;
//using CourseTech.Core.Models;
//using CourseTech.Core.DTOs.Order;
//using CourseTech.Service.Services;
//using Microsoft.Extensions.Logging;

//namespace TestProject2.Services
//{
//    public class OrderServiceTests
//    {
//        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
//        private readonly Mock<IMapper> _mapperMock = new();
//        private readonly Mock<ILogger<OrderService>> _loggerMock = new();
//        private readonly OrderService _orderService;

//        public OrderServiceTests()
//        {
//            _orderService = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
//        }

//        [Fact]
//        public async Task CreateOrderFromBasketAsync_ShouldFail_WhenBasketNotFound()
//        {
//            // Arrange
//            var basketId = Guid.NewGuid();
//            _unitOfWorkMock.Setup(x => x.Basket.GetBasketWithItemsAsync(basketId))
//                .ReturnsAsync((Basket)null);

//            // Act
//            var result = await _orderService.CreateOrderFromBasketAsync(basketId);

//            // Assert
//            Assert.False(result.IsSuccess);
//            _unitOfWorkMock.Verify(x => x.Order.InsertAsync(It.IsAny<Order>()), Times.Never);
//            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
//        }

//        [Fact]
//        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenExists()
//        {
//            // Arrange
//            var orderId = Guid.NewGuid();
//            var order = new Order { Id = orderId };
//            var orderDto = new OrderDTO(orderId, DateTime.UtcNow, "Pending", new List<OrderItemDTO>());

//            _unitOfWorkMock.Setup(x => x.Order.GetOrderByIdWithIncludesAsync(orderId))
//                .ReturnsAsync(order);
//            _mapperMock.Setup(x => x.Map<OrderDTO>(order))
//                .Returns(orderDto);

//            // Act
//            var result = await _orderService.GetOrderByIdAsync(orderId);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(orderDto, result.Data);
//        }

//        [Fact]
//        public async Task GetOrderByIdAsync_ShouldFail_WhenOrderNotFound()
//        {
//            // Arrange
//            var orderId = Guid.NewGuid();
//            _unitOfWorkMock.Setup(x => x.Order.GetOrderByIdWithIncludesAsync(orderId))
//                .ReturnsAsync((Order)null);

//            // Act
//            var result = await _orderService.GetOrderByIdAsync(orderId);

//            // Assert
//            Assert.False(result.IsSuccess);
//        }

//        [Fact]
//        public async Task GetOrdersByUserIdAsync_ShouldReturnUserOrders()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var orders = new List<Order>
//            {
//                new Order { Id = Guid.NewGuid() },
//                new Order { Id = Guid.NewGuid() }
//            };
//            var orderDtos = orders.Select(o => 
//                new OrderDTO(o.Id, DateTime.UtcNow, "Pending", new List<OrderItemDTO>())).ToList();

//            _unitOfWorkMock.Setup(x => x.Order.GetOrdersByUserIdAsync(userId))
//                .ReturnsAsync(orders);
//            _mapperMock.Setup(x => x.Map<List<OrderDTO>>(orders))
//                .Returns(orderDtos);

//            // Act
//            var result = await _orderService.GetOrdersByUserIdAsync(userId);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(orderDtos, result.Data);
//        }
//    }
//}
