namespace MarketplaceBack.DTOs.Request
{
    public class ProcessPaymentRequest
    {
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string UserId { get; set; }
    }
}
