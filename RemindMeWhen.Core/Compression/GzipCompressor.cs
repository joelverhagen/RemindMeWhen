using System;
using System.IO;
using System.IO.Compression;
using Knapcode.RemindMeWhen.Core.Logging;

namespace Knapcode.RemindMeWhen.Core.Compression
{
    public class GzipCompressor : ICompressor
    {
        private readonly IEventSource _eventSource;

        public GzipCompressor(IEventSource eventSource)
        {
            _eventSource = eventSource;
        }

        public byte[] Compress(byte[] decompressed)
        {
            byte[] compressed = null;

            TimeSpan duration = EventTimer.Time(() =>
            {
                var outputStream = new MemoryStream();
                using (var compressStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    compressStream.Write(decompressed, 0, decompressed.Length);
                }
                compressed = outputStream.ToArray();
            });

            _eventSource.OnCompressed(decompressed.LongLength, compressed.LongLength, duration);

            return compressed;
        }

        public byte[] Decompress(byte[] compressed)
        {
            byte[] decompressed = null;

            TimeSpan duration = EventTimer.Time(() =>
            {
                var compressedStream = new MemoryStream(compressed);
                using (var decompressStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    var outputStream = new MemoryStream();
                    decompressStream.CopyTo(outputStream);
                    decompressed = outputStream.ToArray();
                }
            });

            _eventSource.OnDecompressed(compressed.LongLength, decompressed.LongLength, duration);

            return decompressed;
        }
    }
}