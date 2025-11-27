namespace WebAPI.DTOs
{
    public class AutoFilterParams
    {
        public bool isSearch { get; set; }
        public string vinNumber { get; set; }
        public string lotNumber { get; set; }
        public long? client { get; set; }
        public long? auction { get; set; }
        public long? buyAccount { get; set; }
        public string container { get; set; }
        public long? loadPort { get; set; }
        public long? destination { get; set; }
        public long? city { get; set; }
        public int? carId { get; set; }
        public DateTime? deliveryFromDate { get; set; }
        public DateTime? deliveryToDate { get; set; }
        public int? Status { get; set; }
        public DateTime? purchaseFromDate { get; set; }
        public DateTime? purchaseToDate { get; set; }
        public string ClientBuyerName { get; set; }
        public string UpdatePaymentStatus { get; set; }
        public string CarName { get; set; }

    }
}
