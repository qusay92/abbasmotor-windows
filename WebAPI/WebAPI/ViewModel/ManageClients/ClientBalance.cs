namespace WebAPI.ViewModel.ManageClients
{
    public class ClientBalance
    {
        public List<AutoDetails> AutoDetails { get; set; }
        public AutoFooter AutoFooter { get; set; }
    }

    public class AutoDetails
    {
        public long Id { get; set; }
        public int CarCount { get; set; }
        public string Name { get; set; }
        public string VinNo { get; set; }
        public string BuyDateStr { get; set; }
        public string BuyerName { get; set; }
        public decimal Required { get; set; }
        public decimal Paid { get; set; }
        public decimal Total { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SeaFreight { get; set; }
        public decimal InnerFreight { get; set; }
        public decimal Fees { get; set; }
        public decimal PurchaseOrder { get; set; }
        public decimal Commission { get; set; }
        public decimal CustomsClearance { get; set; }
        public decimal StorageFees { get; set; }
        public decimal Other { get; set; }
    }

    public class AutoFooter
    {
        public decimal Required { get; set; }
        public decimal Paid { get; set; }
        public decimal Total { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SeaFreight { get; set; }
        public decimal InnerFreight { get; set; }
        public decimal Fees { get; set; }
        public decimal PurchaseOrder { get; set; }
        public decimal Commission { get; set; }
        public decimal CustomsClearance { get; set; }
        public decimal StorageFees { get; set; }
        public decimal Other { get; set; }
    }
}
