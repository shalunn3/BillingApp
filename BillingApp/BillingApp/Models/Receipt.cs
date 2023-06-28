namespace BillingApp.Models
{
    public class Receipt
    {
        public string? OrderNumber { get; set; }

        public decimal PayableAmount { get; set; }

        public bool PaymentSuccessful { get; set; }
    }
}