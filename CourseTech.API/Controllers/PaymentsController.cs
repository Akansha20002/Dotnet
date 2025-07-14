using CourseTech.Core.DTOs.Payment;
using CourseTech.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseTech.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IPaymentService service) : CustomBaseController
    {
        [HttpPost]
        public async Task<IActionResult> ProcessPaymentAsync([FromBody] PaymentRequestDTO paymentRequest)
        {
            var result = await service.ProcessPaymentAsync(paymentRequest);
            return CreateActionResult(result);
        }

        [HttpGet("{userId:guid}/user")]
        public async Task<IActionResult> GetPaymentsByUserId(Guid userId)
        {
            var result = await service.GetPaymentsByUserAsync(userId);
            return CreateActionResult(result);
        }

        [HttpGet("{paymentId:guid}")]
        public async Task<IActionResult> GetPaymentById(Guid paymentId)
        {
            var result = await service.GetPaymentByIdAsync(paymentId);
            return CreateActionResult(result);
        }

    }
}