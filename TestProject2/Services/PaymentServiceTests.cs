//using Xunit;
//using Moq;
//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using AutoMapper;
//using CourseTech.Core.Services;
//using CourseTech.Core.UnitOfWorks;
//using CourseTech.Core.Models;
//using CourseTech.Core.DTOs.Payment;
//using CourseTech.Service.Services;

//namespace TestProject2.Services
//{
//    public class PaymentServiceTests
//    {
//        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
//        private readonly Mock<IMapper> _mapperMock = new();
//        private readonly PaymentService _paymentService;

//        public PaymentServiceTests()
//        {
//            _paymentService = new PaymentService(_unitOfWorkMock.Object, _mapperMock.Object);
//        }

//        [Fact]
//        public async Task GetPaymentByIdAsync_ShouldReturnPayment_WhenExists()
//        {
//            // Arrange
//            var paymentId = Guid.NewGuid();
//            var payment = new Payment(Guid.NewGuid(), Guid.NewGuid(), 100m, "USD", "Transaction1");
//            var paymentDto = new PaymentDTO(paymentId, Guid.NewGuid(), Guid.NewGuid(), 
//                100m, "USD", "Transaction1", DateTime.UtcNow, "Completed");

//            _unitOfWorkMock.Setup(x => x.Payment.GetByIdAsync(paymentId))
//                .ReturnsAsync(payment);
//            _mapperMock.Setup(x => x.Map<PaymentDTO>(payment))
//                .Returns(paymentDto);

//            // Act
//            var result = await _paymentService.GetPaymentByIdAsync(paymentId);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(paymentDto, result.Data);
//        }

//        [Fact]
//        public async Task GetPaymentByIdAsync_ShouldFail_WhenPaymentNotFound()
//        {
//            // Arrange
//            var paymentId = Guid.NewGuid();
//            _unitOfWorkMock.Setup(x => x.Payment.GetByIdAsync(paymentId))
//                .ReturnsAsync((Payment)null);

//            // Act
//            var result = await _paymentService.GetPaymentByIdAsync(paymentId);

//            // Assert
//            Assert.False(result.IsSuccess);
//        }

//        [Fact]
//        public async Task GetPaymentsByUserAsync_ShouldReturnUserPayments()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var payments = new List<Payment>
//            {
//                new Payment(userId, Guid.NewGuid(), 100m, "USD", "Transaction1"),
//                new Payment(userId, Guid.NewGuid(), 200m, "USD", "Transaction2")
//            };
//            var paymentDtos = payments.Select(p => new PaymentDTO(
//                p.Id, p.UserId, p.OrderId, p.Amount, p.Currency, 
//                p.TransactionId, p.CreatedAt, "Completed")).ToList();

//            _unitOfWorkMock.Setup(x => x.Payment.GetByUserIdAsync(userId))
//                .ReturnsAsync(payments);
//            _mapperMock.Setup(x => x.Map<List<PaymentDTO>>(payments))
//                .Returns(paymentDtos);

//            // Act
//            var result = await _paymentService.GetPaymentsByUserAsync(userId);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(paymentDtos, result.Data);
//        }

//        [Fact]
//        public async Task ProcessPaymentAsync_ShouldFail_WhenOrderNotFound()
//        {
//            // Arrange
//            var paymentRequest = new PaymentRequestDTO(Guid.NewGuid(), "Card", "4111111111111111", 
//                "12/25", "123", "John Doe");

//            _unitOfWorkMock.Setup(x => x.Order.GetOrderByIdWithIncludesAsync(paymentRequest.OrderId))
//                .ReturnsAsync((Order)null);

//            // Act
//            var result = await _paymentService.ProcessPaymentAsync(paymentRequest);

//            // Assert
//            Assert.False(result.IsSuccess);
//        }
//    }
//}
