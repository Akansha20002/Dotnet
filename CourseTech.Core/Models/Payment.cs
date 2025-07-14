using CourseTech.Shared.Enums;

namespace CourseTech.Core.Models
{
    public class Payment
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public AppUser AppUser { get; private set; } 

        public Guid OrderId { get; private set; }
        public Order Order { get; private set; }

        public decimal TotalAmount { get; private set; }
        public string PaymentProvider { get; private set; }
        public string TransactionId { get; private set; } = string.Empty;
        public bool IsSuccessful { get; private set; }
        public DateTime PaymentDate { get; private set; } = DateTime.UtcNow;
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

        private Payment() { }
        public Payment(Guid userId, Guid orderId, Order order, decimal totalAmount, string paymentProvider)
        {
            UserId = userId;
            OrderId = orderId;
            Order = order;
            TotalAmount = totalAmount;
            PaymentProvider = paymentProvider;
            TransactionId = string.Empty;
            IsSuccessful = false;
            Status = PaymentStatus.Pending;
            PaymentDate = DateTime.UtcNow;
        }

        public void MarkAsSuccessful(string transactionId)
        {
            TransactionId = transactionId;
            Status = PaymentStatus.Success;
            IsSuccessful = true;
        }

        public void MarkAsFailed()
        {
            Status = PaymentStatus.Failed;
            IsSuccessful = false;
        }
    }
}