using System.IO;
using System.IO.Compression;
using Knapcode.RemindMeWhen.Core.Logging;
using Knapcode.RemindMeWhen.Core.Support;

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
            var compressedLength = new Reference<long>();
            using (EventTimer.OnCompletion(d => _eventSource.OnCompressed(decompressed.LongLength, compressedLength.Value, d)))
            {
                var outputStream = new MemoryStream();
                using (var compressStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    compressStream.Write(decompressed, 0, decompressed.Length);
                }
                byte[] compressed = outputStream.ToArray();
                compressedLength.Value = compressed.LongLength;
                return compressed;
            }
        }

        public byte[] Decompress(byte[] compressed)
        {
            var decompressedLength = new Reference<long>();
            using (EventTimer.OnCompletion(d => _eventSource.OnDecompressed(compressed.LongLength, decompressedLength.Value, d)))
            {
                var compressedStream = new MemoryStream(compressed);
                using (var decompressStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    var outputStream = new MemoryStream();
                    decompressStream.CopyTo(outputStream);
                    byte[] decompressed = outputStream.ToArray();
                    decompressedLength.Value = decompressed.LongLength;
                    return decompressed;
                }
            }
        }
    }
}