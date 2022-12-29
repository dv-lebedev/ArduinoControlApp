/*
Copyright(c) 2022-2023 Denis Lebedev
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using ArduinoControlApp.Interfaces;
using System;
using System.IO.Ports;

namespace ArduinoControlApp.Serial
{
    internal class ComPortDevice : IDevice
    {
        readonly SerialPort     _port;
        bool                    _isOpened;

        public bool IsOpened
        {
            get => _isOpened;

            private set
            {
                _isOpened = value;
            }
        }

        public event EventHandler Opened;
        public event EventHandler Closed;

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

        public void Open()
        {
            _port.Open();
            IsOpened = true;
            Opened?.Invoke(this, EventArgs.Empty);
        }

        public void Close()
        {
            _port.Close();
            IsOpened = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }
        
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

        public void Dispose()
        {
            _port.Dispose();

            if (IsOpened)
            {
                IsOpened = false;
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
