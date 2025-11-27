namespace Entities
{
    public partial class PaymentDetails
    {
        public long Id { get; set; }
        public int CashType { get; set; }
        public string Notes { get; set; }
        public DateTime PayDate { get; set; }
        public long PaymentId { get; set; }
        public decimal Amount { get; set; }
        public BuyType BuyType { get; set; }
        public Payment Payment { get; set; }
    }
}
