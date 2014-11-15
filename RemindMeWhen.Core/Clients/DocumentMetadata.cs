using System;
using Knapcode.RemindMeWhen.Core.Identities;

namespace Knapcode.RemindMeWhen.Core.Clients
{
    public class DocumentMetadata
    {
        public DocumentIdentity Identity { get; set; }
        public string Hash { get; set; }
        public DateTime LastPersisted { get; set; }
    }
}