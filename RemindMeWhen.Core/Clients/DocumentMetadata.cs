using System;

namespace Knapcode.RemindMeWhen.Core.Clients
{
    public class DocumentMetadata
    {
        public string Id { get; set; }
        public DocumentId DocumentId { get; set; }
        public string Hash { get; set; }
        public bool Duplicate { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}