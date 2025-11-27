using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public partial class LookupValue
    {
        [NotMapped]
        public string LookupName { get; set; }
    }
}
