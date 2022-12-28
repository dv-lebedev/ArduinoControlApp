using ComPortApp.Monitor;
using System;

namespace ComPortApp.Models
{
    internal class SerialPortDeviceModel : DeviceModel
    {
        private string _portName;
        private int _baudRate;

        public string PortName
        {
            get => _portName;

            set
            {
                if (_portName != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException(nameof(PortName));
                    }

                    _portName = value;
                }
            }
        }

        public int Baudrate
        {
            get => _baudRate;

            set
            {
                if (value != _baudRate)
                {
                    if (value <= 0)
                    {
                        throw new ArgumentException(nameof(Baudrate));
                    }

                    _baudRate = value;
                }
            }
        }

        protected override void InitCurrentDevice()
        {
            CurrentDevice = new ComPortDevice(PortName, Baudrate);
            CurrentDeviceModel = this;
        }

        public string[] GetAvailablePorts()
        {
            return ComPortDevice.GetAvailable();
        }
    }

}
