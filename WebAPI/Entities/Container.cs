namespace Entities
{
    public partial class Container : BaseModel
    {

        public Container()
        {
            Autos = new HashSet<Auto>();
            Images = new HashSet<ContainerImages>();
        }

        public string Name { get; set; }
        public string BookNo { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public long? DeparturePortId { get; set; }
        public long? DestinationId { get; set; }
        public byte? IsArchive { get; set; }
        public long? ShippingCompanyId { get; set; }
        public int? Status { get; set; }

        public virtual LookupValue DeparturePort { get; set; }
        public virtual LookupValue Destination { get; set; }
        public virtual LookupValue ShippingCompany { get; set; }
        public virtual ICollection<Auto> Autos { get; set; }
        public virtual ICollection<ContainerImages> Images { get; set; }
    }
}
