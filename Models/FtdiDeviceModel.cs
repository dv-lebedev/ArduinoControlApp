using ComPortApp.Ftdi;
using ComPortApp.Monitor;
using System;

namespace ComPortApp.Models
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
