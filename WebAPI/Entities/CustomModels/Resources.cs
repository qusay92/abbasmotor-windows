using Entities.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public partial class Resources
    {
        [NotMapped]
        public string Text { get { return new LocalizedString(TextAr, TextEn).Current; } }
    }
}
