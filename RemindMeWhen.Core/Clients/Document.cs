using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Clients
{
    public class Document
    {
        public DocumentId Id { get; set; }
        public byte[] Content;
    }
}
