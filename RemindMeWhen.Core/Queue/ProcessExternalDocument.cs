using Knapcode.RemindMeWhen.Core.Clients;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public class ProcessExternalDocument
    {
        public ExternalDocumentType Type { get; set; }
        public string Identitity { get; set; }
    }
}