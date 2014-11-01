using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public interface IEvent<out TContent> : IEvent
    {
        TContent Content { get; }
    }
}
