using ArduinoControlApp.Ftdi;
using ArduinoControlApp.Monitor;
using System;

namespace ArduinoControlApp.Models
{
    internal class FtdiDeviceModel : DeviceModel
    {
        private string _serialNumber;

        public string SerialNumber
        {
            get => _serialNumber;
            set
            {
                _serialNumber = value ?? throw new ArgumentException(nameof(SerialNumber));
            }
        }

        protected override void InitCurrentDevice()
        {
            var ftdi = new FtdiDevice
            {
                SerialNumber = SerialNumber
            };

            CurrentDevice = ftdi;
            CurrentDeviceModel = this;
        }

        public string[] GetAvailableSerialNumbers()
        {
            return FtdiDevice.GetAvailableSerialNumbers();
        }
    }
}
