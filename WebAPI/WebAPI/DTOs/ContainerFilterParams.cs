namespace WebAPI.DTOs
{
    public class ContainerFilterParams
    {
        public bool IsSearch { get; set; }
        public int? StatusId { get; set; }
        public string ContainerNo { get; set; }
        public string BookingNo { get; set; }
        public DateTime? LoadingFromDate { get; set; }
        public DateTime? LoadingToDate { get; set; }
        public int? LoadPortId { get; set; }
        public int? DestinationId { get; set; }
        public int? ShippingLineId { get; set; }
        public int? ClientId { get; set; }
        public DateTime? ArrivalFromDate { get; set; }
        public DateTime? ArrivalToDate { get; set; }
    }
}
