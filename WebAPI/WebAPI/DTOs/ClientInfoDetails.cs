namespace WebAPI.DTOs
{
    public class ClientPaymentDetails
    {
        public Header Header { get; set; }
        public List<Details> Details { get; set; }
        public Footer Footer { get; set; }
        public string ClientName { get; set; }
    }

    public class Header
    {
        public decimal Required { get; set; }
        public decimal Payed { get; set; }
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

    public class Details
    {
        public decimal Required { get; set; }
        public decimal Payed { get; set; }
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
        public DateTime? ArrivalDate { get; set; }
        public DateTime BuyDate { get; set; }
        public string Destination { get; set; }
        public string Container { get; set; }
        public string LoadPort { get; set; }
        public string City { get; set; }
        public string Lot { get; set; }
        public string Vin { get; set; }
        public string Car { get; set; }
    }

    public class Footer
    {
        public decimal Required { get; set; }
        public decimal Payed { get; set; }
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
