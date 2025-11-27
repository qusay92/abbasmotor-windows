namespace WebAPI.ViewModel.Payments
{
    public class PaymentInput
    {
        public long id { get; set; }
        public int paymentType { get; set; }
        public decimal amount { get; set; }
        public long autoId { get; set; }
        public long categoryId { get; set; }
        public DateTime payDate { get; set; }
        public int paymentMethod { get; set; }
        public string notes { get; set; }

        public int? PaymentId { get; set; }
    }
}
