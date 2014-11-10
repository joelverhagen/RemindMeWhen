namespace Knapcode.RemindMeWhen.Core.Clients
{
    public interface IDeserializer<out T>
    {
        T Deserialize(byte[] content);
    }
}