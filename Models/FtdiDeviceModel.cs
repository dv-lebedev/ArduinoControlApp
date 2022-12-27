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
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(nameof(SerialNumber));
                }

                _serialNumber = value;
            }
        }

        protected override void InitCurrentDevice()
        {
            CurrentDevice = new FtdiDevice(SerialNumber);
        }
    }
}
