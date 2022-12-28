using ArduinoDecoder;

namespace ArduinoControlApp.Interfaces
{
    public interface IDataConsumer
    {
        void Consume(Package package);
    }
}
