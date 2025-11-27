namespace WebAPI.ViewModel.Balances
{
    public class GetBalancesInput
    {
        public bool IsSearch { get; set; }
        public int? AutoId { get; set; }
        public string VinNo { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public int? ClientId { get; set; }
    }
}
