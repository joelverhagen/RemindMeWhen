using System;
using System.Diagnostics;

namespace Knapcode.RemindMeWhen.Core.Logging
{
    public class EventTimer : IDisposable
    {
        private readonly Action<TimeSpan> _onCompletion;
        private readonly Stopwatch _stopwatch;

        private EventTimer(Action<TimeSpan> onCompletion)
        {
            _onCompletion = onCompletion;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _onCompletion(_stopwatch.Elapsed);
        }

        public static EventTimer OnCompletion(Action<TimeSpan> onCompletion)
        {
            return new EventTimer(onCompletion);
        }
    }
}