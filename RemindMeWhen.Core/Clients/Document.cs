using Knapcode.RemindMeWhen.Core.Identities;

namespace Knapcode.RemindMeWhen.Core.Clients
{
    public class Document
    {
        public DocumentIdentity Identity { get; set; }
        public byte[] Content { get; set; }
    }
}