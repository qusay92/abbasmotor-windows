using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Shared
{
    public class BaseModel
    {
        public long Id { get; set; }
        public long? CreationUserId { get; set; }  
        public DateTime? CreationDate { get; set; }
        public long? ModificationUserId { get; set; }
        public DateTime? ModificationDate { get; set; }
        public virtual User CreationUser { get; set; }
        public virtual User ModificationUser { get; set; }
    }
}
