namespace Entities
{
    public partial class LookupValue
    {
        public LookupValue()
        {
            AutoAuctions = new HashSet<Auto>();
            AutoBrands = new HashSet<Auto>();
            AutoBuyingAccounts = new HashSet<Auto>();
            AutoCities = new HashSet<Auto>();
            AutoDestinations = new HashSet<Auto>();
            AutoLoadPorts = new HashSet<Auto>();
            ContainerDeparturePorts = new HashSet<Container>();
            ContainerDestinations = new HashSet<Container>();
            ContainerShippingCompanies = new HashSet<Container>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long LookupId { get; set; }

        public virtual Lookup Lookup { get; set; }
        public virtual ICollection<Auto> AutoAuctions { get; set; }
        public virtual ICollection<Auto> AutoBrands { get; set; }
        public virtual ICollection<Auto> AutoBuyingAccounts { get; set; }
        public virtual ICollection<Auto> AutoCities { get; set; }
        public virtual ICollection<Auto> AutoDestinations { get; set; }
        public virtual ICollection<Auto> AutoLoadPorts { get; set; }
        public virtual ICollection<Container> ContainerDeparturePorts { get; set; }
        public virtual ICollection<Container> ContainerDestinations { get; set; }
        public virtual ICollection<Container> ContainerShippingCompanies { get; set; }

    }
}
