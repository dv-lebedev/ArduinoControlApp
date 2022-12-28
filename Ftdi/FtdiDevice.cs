using ArduinoControlApp.Interfaces;
using ArduinoControlApp.Monitor;
using FTD2XX_NET;
using System;
using System.Linq;

namespace ArduinoControlApp.Ftdi
{
    internal class FtdiDevice : IDevice
    {
        private string _serialNumber;
        private readonly FTDI ftdi = new FTDI();
        private readonly static FTDI _detector = new FTDI();

        public string SerialNumber
        {
            get => _serialNumber;

            set
            {
                _serialNumber = value ?? throw new ArgumentException(nameof(SerialNumber));
            }
        }

        public FtdiDevice()
        {
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            ftdi.Close();
        }

        public void Open()
        {
            ftdi.OpenBySerialNumberExtended2(SerialNumber);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            uint numBytesRead = 0;

            if (count > 0)
            {
                ftdi.Read(buffer, (uint)count, ref numBytesRead);
            }

            return (int)numBytesRead;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            uint numBytesWritten = 0;
            ftdi?.Write(buffer, count, ref numBytesWritten);
        }

        public static string[] GetAvailableSerialNumbers()
        {
            FTDI.FT_DEVICE_INFO_NODE[] nodes = new FTDI.FT_DEVICE_INFO_NODE[1024];

            if (_detector.GetDeviceList(nodes) == FTDI.FT_STATUS.FT_OK)
            {
                return nodes
                    .Where(i => !string.IsNullOrEmpty(i?.SerialNumber))
                    .Select(i => i.SerialNumber)
                    .ToArray();
            }

            return new string[0];
        }
    }
}
