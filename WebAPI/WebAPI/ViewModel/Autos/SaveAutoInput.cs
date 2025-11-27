namespace WebAPI.ViewModel.Autos
{
    public class SaveAutoInput
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long BrandId { get; set; }
        public string Description { get; set; }
        public int Model { get; set; }
        public string VinNo { get; set; }
        public int Type { get; set; }
        public long Color { get; set; }
        public string Engine { get; set; }
        public string Lot { get; set; }
        public int CarType { get; set; }
        public string CarName { get; set; }
        public long? BuyingAccountId { get; set; }
        public decimal? RemainingPayment { get; set; }
        public long? BuyerId { get; set; }
        public long LoadPortId { get; set; }
        public long AuctionId { get; set; }
        public long CityId { get; set; }
        public long DestinationId { get; set; }
        public long? ContainerId { get; set; }
        public DateTime BuyDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public int CarStatus { get; set; }
        public int? PaperStatus { get; set; }
        public int DisplayStatus { get; set; }
        public byte IsArchive { get; set; }

        public long? CreationUserId { get; set; }
        public DateTime? CreationDate { get; set; }
        public long? ModificationUserId { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string ClientBuyerName { get; set; }
        public string UpdatePaymentStatus { get; set; }
    }
}
