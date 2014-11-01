using System;

namespace Knapcode.RemindMeWhen.WebApi.Models
{
    public class PageLinks
    {
        public Uri Previous { get; set; }
        public Uri Self { get; set; }
        public Uri Next { get; set; }
    }
}