using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public partial class Container
    {
        [NotMapped]
        public string DeparturePortName { get; set; }

        [NotMapped]
        public string DestinationName { get; set; }

        [NotMapped]
        public string ShippingCompanyName { get; set; }

        [NotMapped]
        public string ContainerStatusStr { get; set; }

        [NotMapped]
        public string RowColor { get; set; }

        [NotMapped]
        public string Image { get; set; }

        [NotMapped]
        public List<long> AutoIds { get; set; }

        [NotMapped]
        public List<Hook<long, string>> AutoVinNos { get; set; }

        [NotMapped]
        public string DepartureDateStr { get; set; }

        [NotMapped]
        public string ArrivalDateStr { get; set; }

        [NotMapped]
        public DateTime? SearchDepartureDate { get; set; }

        [NotMapped]
        public DateTime? SearchArrivalDate { get; set; }
        
    }
}
