using ArduinoDecoder;

namespace ComPortApp.Monitor
{
    public interface IDataConsumer
    {
        void Consume(Package package);
    }
}
