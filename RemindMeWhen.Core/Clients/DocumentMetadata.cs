using System;

namespace Knapcode.RemindMeWhen.Core.Clients
{
    public class DocumentMetadata
    {
        public DocumentType Type { get; set; }
        public string TypeIdentity { get; set; }
        public string Hash { get; set; }
        public DateTime LastPersisted { get; set; }
    }
}