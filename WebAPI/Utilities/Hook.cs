using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Hook<T1, T2>
    {
        public T1 Value { get; set; }
        public T2 Text { get; set; }
    }

    public class StatusHook<T1, T2>
    {
        public T1 Status { get; set; }
        public T2 Color { get; set; }
    }
}
