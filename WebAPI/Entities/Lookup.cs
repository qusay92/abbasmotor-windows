namespace Entities
{
    public partial class Lookup
    {
        public Lookup()
        {
            LookupValues = new HashSet<LookupValue>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<LookupValue> LookupValues { get; set; }
    }
}
