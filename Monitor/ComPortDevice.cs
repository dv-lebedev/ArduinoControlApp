using ComPortApp.Monitor;
using System;
using System.IO.Ports;

namespace ComPortApp
{
    internal class ComPortDevice : IDevice
    {
        private readonly SerialPort _port;

        public ComPortDevice(string portName, int baudrate) 
        {
            if (string.IsNullOrEmpty(portName))
            {
                throw new ArgumentException(nameof(portName));
            }

            if (baudrate < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baudrate));
            }

            _port = new SerialPort(portName, baudrate);
            _port.DtrEnable = true;
            _port.RtsEnable = true;
        }

        public void Open() => _port.Open();
        
        public void Dispose() => _port.Dispose();
        
        public void Close() =>_port.Close();
        
        public int Read(byte[] buffer, int offset, int count)
        {
            if (_port.BytesToRead > 0)
            {
                return _port.Read(buffer, offset, count);
            }

            return 0;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _port?.Write(buffer, offset, count);
        }

        internal static string[] GetAvailable()
        {
            return SerialPort.GetPortNames();
        }
    }
}
