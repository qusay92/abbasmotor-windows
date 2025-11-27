namespace Entities
{
    public partial class Payment : BaseModel
    {

        public Payment()
        {
            PaymentDetails = new HashSet<PaymentDetails>();
        }

        public PaymentType PaymentType { get; set; }
        public decimal TotalAmount { get; set; }
        public long AutoId { get; set; }
        public virtual Auto Auto { get; set; }

        public virtual ICollection<PaymentDetails> PaymentDetails { get; set; }
    }
}
