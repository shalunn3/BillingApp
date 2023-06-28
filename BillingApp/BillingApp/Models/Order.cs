namespace BillingApp.Models
{
    public class Order
    {
        public string? OrderNumber { get; set; }

        public int UserId { get; set; }

        public decimal PayableAmount { get; set; }

        public int PaymentGateway { get; set; }

        public string? OptionalDescription { get; set; }
    }
}