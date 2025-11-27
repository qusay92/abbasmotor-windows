namespace WebAPI.ViewModel.Payments
{
    public class GetPaymentsInput
    {
        public bool IsSearch { get; set; }
        public int? AutoId { get; set; }
        public string VinNo { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public int? ClientId { get; set; }
    }
}
