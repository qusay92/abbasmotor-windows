using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.Enums.Enum;

namespace Utilities
{
    public class ResponseResult<T>
    {
        public T Data { get; set; }
        public StatusType Status { get; set; }
        public List<string> Errors { get; set; }
    }
}
