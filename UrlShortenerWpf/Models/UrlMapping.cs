using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerWpf.Models
{
    public class UrlMapping
    {
        public string ShortKey { get; set; }
        public string LongUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiryAt { get; set; }

    }
}
