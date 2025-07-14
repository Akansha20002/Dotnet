using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.Payment;
using CourseTech.Shared.Enums;
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
    public class PaymentTest
    {
        private Mock<IPaymentService> _serviceMock;
        private PaymentsController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IPaymentService>();
            _controller = new PaymentsController(_serviceMock.Object);
        }

        #region ProcessPaymentAsync

        [Test]
        public async Task ProcessPaymentAsync_ReturnsOk_WhenSuccessful()
        {
            var request = new PaymentRequestDTO(
                Guid.NewGuid(), "John Doe", "4111111111111111", "12", "2025", "123", 150.00m, DateTime.UtcNow);

            var dto = new PaymentDTO(
                Guid.NewGuid(), Guid.NewGuid(), request.OrderId, "Visa", request.TotalAmount, "TXN123",
                true, DateTime.UtcNow, PaymentStatus.Success);

            var result = ServiceResult<PaymentDTO>.Success(dto);

            _serviceMock.Setup(s => s.ProcessPaymentAsync(request)).ReturnsAsync(result);

            var response = await _controller.ProcessPaymentAsync(request);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task ProcessPaymentAsync_ReturnsBadRequest_WhenFailed()
        {
            var request = new PaymentRequestDTO(
                Guid.NewGuid(), "John Doe", "4111111111111111", "12", "2025", "123", 150.00m, DateTime.UtcNow);

            var result = ServiceResult<PaymentDTO>.Fail("Insufficient funds");

            _serviceMock.Setup(s => s.ProcessPaymentAsync(request)).ReturnsAsync(result);

            var response = await _controller.ProcessPaymentAsync(request);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task ProcessPaymentAsync_ThrowsException_Returns500()
        {
            var request = new PaymentRequestDTO(
                Guid.NewGuid(), "John Doe", "4111111111111111", "12", "2025", "123", 150.00m, DateTime.UtcNow);

            _serviceMock.Setup(s => s.ProcessPaymentAsync(request))
                .ThrowsAsync(new Exception("Gateway unavailable"));

            Assert.ThrowsAsync<Exception>(() => _controller.ProcessPaymentAsync(request));
        }

        #endregion

        #region GetPaymentsByUserId

        [Test]
        public async Task GetPaymentsByUserId_ReturnsOk_WithPayments()
        {
            var userId = Guid.NewGuid();
            var payments = new List<PaymentDTO>
            {
                new PaymentDTO(Guid.NewGuid(), userId, Guid.NewGuid(), "Stripe", 99.99m,
                "TXN001", true, DateTime.UtcNow, PaymentStatus.Success)
            };

            var result = ServiceResult<List<PaymentDTO>>.Success(payments);

            _serviceMock.Setup(s => s.GetPaymentsByUserAsync(userId)).ReturnsAsync(result);

            var response = await _controller.GetPaymentsByUserId(userId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task GetPaymentsByUserId_ReturnsBadRequest_WhenNotFound()
        {
            var userId = Guid.NewGuid();
            var result = ServiceResult<List<PaymentDTO>>.Fail("No payments found");

            _serviceMock.Setup(s => s.GetPaymentsByUserAsync(userId)).ReturnsAsync(result);

            var response = await _controller.GetPaymentsByUserId(userId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task GetPaymentsByUserId_ReturnsOk_WithEmptyList()
        {
            var userId = Guid.NewGuid();
            var result = ServiceResult<List<PaymentDTO>>.Success(new List<PaymentDTO>());

            _serviceMock.Setup(s => s.GetPaymentsByUserAsync(userId)).ReturnsAsync(result);

            var response = await _controller.GetPaymentsByUserId(userId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        #endregion

        #region GetPaymentById

        [Test]
        public async Task GetPaymentById_ReturnsOk_WhenFound()
        {
            var paymentId = Guid.NewGuid();
            var dto = new PaymentDTO(
                paymentId, Guid.NewGuid(), Guid.NewGuid(), "Razorpay", 199.99m,
                "TXN999", true, DateTime.UtcNow, PaymentStatus.Success);

            var result = ServiceResult<PaymentDTO>.Success(dto);

            _serviceMock.Setup(s => s.GetPaymentByIdAsync(paymentId)).ReturnsAsync(result);

            var response = await _controller.GetPaymentById(paymentId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(200, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        [Test]
        public async Task GetPaymentById_ReturnsBadRequest_WhenNotFound()
        {
            var paymentId = Guid.NewGuid();
            var result = ServiceResult<PaymentDTO>.Fail("Payment not found");

            _serviceMock.Setup(s => s.GetPaymentByIdAsync(paymentId)).ReturnsAsync(result);

            var response = await _controller.GetPaymentById(paymentId);

            Assert.IsInstanceOf<ObjectResult>(response);
            var objectResult = response as ObjectResult;
            Assert.AreEqual(400, objectResult!.StatusCode);
            Assert.AreEqual(result, objectResult.Value);
        }

        #endregion
    }
}
