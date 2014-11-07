using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Settings
{
    public class AzureQueueSettings
    {
        public IDictionary<string, string> QueueNames { get; set; }
    }
}
