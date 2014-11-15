using System;

namespace Knapcode.RemindMeWhen.Core.Clients
{
    public class DocumentMetadata
    {
        public Guid Id { get; set; }
        public DocumentId DocumentId { get; set; }
        public string Hash { get; set; }
        public bool Duplicate { get; set; }
        public DateTime Created { get; set; }
    }
}