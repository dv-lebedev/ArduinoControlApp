using ComPortApp.Monitor;
using FTD2XX_NET;
using System;

namespace ComPortApp.Ftdi
{
    internal class FtdiDevice : IDevice
    {
        private readonly FTDI ftdi = new FTDI();

        public string SerialNumber { get; private set; }

        public FtdiDevice(string serialNumber)
        {
            SerialNumber = serialNumber ?? throw new ArgumentNullException(nameof(serialNumber));
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
    }
}
