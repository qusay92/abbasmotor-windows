using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entities
{
    public class AutoImage : BaseModel
    {
        public long AutoId { get; set; }
        public string Alt { get; set; }
        public string Extintion { get; set; }
        public string Path { get; set; }
        public string PreviewImageSrc { get; set; }
        public string ThumbnailImageSrc { get; set; }
        public string Title { get; set; }

        public virtual Auto Auto { get; set; }     
    }
}
