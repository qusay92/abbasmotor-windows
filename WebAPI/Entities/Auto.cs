namespace Entities
{
    public partial class Auto : BaseModel
    {
        public Auto()
        {
            Images = new HashSet<AutoImage>();
            Payments = new HashSet<Payment>();
        }
        public string Name { get; set; }
        public long BrandId { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public string VinNo { get; set; }
        public int Type { get; set; }
        public long ColorId { get; set; }
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
        public string ClientBuyerName { get; set; }
        public string UpdatePaymentStatus { get; set; }


        public virtual LookupValue Auction { get; set; }
        public virtual LookupValue Brand { get; set; }
        public virtual User Buyer { get; set; }
        public virtual LookupValue BuyingAccount { get; set; }
        public virtual LookupValue City { get; set; }
        public virtual Container Container { get; set; }
        public virtual LookupValue Destination { get; set; }
        public virtual LookupValue LoadPort { get; set; }
        public virtual ICollection<AutoImage> Images { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
