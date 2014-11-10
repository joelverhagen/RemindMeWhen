namespace Knapcode.RemindMeWhen.Core.Compression
{
    public interface ICompressor
    {
        byte[] Compress(byte[] decompressed);
        byte[] Decompress(byte[] compressed);
    }
}