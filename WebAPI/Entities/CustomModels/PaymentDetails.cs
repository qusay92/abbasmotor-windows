using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public partial class PaymentDetails
    {
        [NotMapped]
        public string CashTypeStr { get; set; }
        [NotMapped]
        public string BuyTypeStr { get; set; }
        [NotMapped]
        public decimal DebitAmount { get; set; }
        
        [NotMapped]
        public long AutoId { get; set; }

        [NotMapped]
        public string VinNo { get; set; }

        [NotMapped]
        public string Client { get; set; }

        [NotMapped]
        public string PayDateStr { get; set; }
       
        [NotMapped]
        public string CarName { get; set; }
       
        [NotMapped]
        public int PaymentType { get; set; }
        

    }
}
