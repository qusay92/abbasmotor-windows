using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public partial class Auto
    {
        [NotMapped]
        public string BrandName { get; set; }
        [NotMapped]
        public string BuyerName { get; set; }
        [NotMapped]
        public string BuyerUsername { get; set; }
        [NotMapped]
        public string BuyerMobile { get; set; }
        [NotMapped]
        public string LoadPortName { get; set; }
        [NotMapped]
        public string AuctionName { get; set; }
        [NotMapped]
        public string CityName { get; set; }
        [NotMapped]
        public string DestinationName { get; set; }
        [NotMapped]
        public string BuyAccountName { get; set; }
        [NotMapped]
        public string ContainerSerial { get; set; }
        [NotMapped]
        public string DisplayStatusStr { get; set; }
        [NotMapped]
        public string CarStatusStr { get; set; }
        [NotMapped]
        public string PaperStatusStr { get; set; }
        [NotMapped]
        public string TypeStr { get; set; }
        [NotMapped]
        public string RowColor { get; set; }
        [NotMapped]
        public string CarStr { get; set; }
        [NotMapped]
        public string Image { get; set; }
        [NotMapped]
        public string BookNo { get; set; }
        [NotMapped]
        public DateTime? DepartureDate { get; set; }
        [NotMapped]
        public string ShippingCompany { get; set; }
        
        [NotMapped]
        public string  Color { get; set; }

        [NotMapped]
        public string BuyDateStr { get; set; }
        
        [NotMapped]
        public string DeliveredDateStr { get; set; }
       
        [NotMapped]
        public DateTime? ContainerArrivalDate { get; set; }

        [NotMapped]
        public string ContainerArrivalDateStr { get; set; }
       
        [NotMapped]
        public string DepartureDateStr { get; set; }

        [NotMapped]
        public int ArchivedAuto { get; set; }
    }
}
