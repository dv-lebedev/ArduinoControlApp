using ComPortApp.Monitor;

namespace ComPortApp.Models
{
    internal class SerialPortDeviceModel : DeviceModel
    {
        public string PortName { get; set; }
        public int Baudrate { get; set; }

        protected override void InitCurrentDevice()
        {
            CurrentDevice = new ComPortDevice(PortName, Baudrate);
        }
    }

}
