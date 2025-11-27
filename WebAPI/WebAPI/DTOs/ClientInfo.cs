namespace WebAPI.DTOs
{
    public class ClientInfo
    {
        public List<InfoDetails> AutoDetails { get; set; }
        public InfoFooter Summation { get; set; }

        public List<InfoDetails> OpenAutos { get; set; }
        public List<InfoDetails> PaidAutos { get; set; }
        public InfoFooter OpenSummation { get; set; }
        public InfoFooter PaidSummation { get; set; }


    }

    public class InfoDetails
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string VinNo { get; set; }
        public string BuyDateStr { get; set; }
        public string BuyerName { get; set; }
        public decimal? Required { get; set; }
        public decimal? Paid { get; set; }
        public decimal? Total { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SeaFreight { get; set; }
        public decimal? InnerFreight { get; set; }
        public decimal? Fees { get; set; }
        public decimal? PurchaseOrder { get; set; }
        public decimal? Commission { get; set; }
        public decimal? CustomsClearance { get; set; }
        public decimal? StorageFees { get; set; }
        public decimal? Other { get; set; }
    }

    public class InfoFooter
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
