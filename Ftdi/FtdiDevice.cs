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
using FTD2XX_NET;
using System;
using System.Linq;

namespace ArduinoControlApp.Ftdi
{
    internal class FtdiDevice : IDevice
    {
        string                  _serialNumber;
        readonly FTDI           _ftdi = new FTDI();
        readonly static FTDI    _detector = new FTDI();
        bool                    _isOpened;

        public string SerialNumber
        {
            get => _serialNumber;

            set
            {
                _serialNumber = value ?? throw new ArgumentException(nameof(SerialNumber));
            }
        }

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

        public FtdiDevice()
        {
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_ftdi.Close() == FTDI.FT_STATUS.FT_OK)
            {
                IsOpened = false;
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Open()
        {
            if (_ftdi.OpenBySerialNumberExtended2(SerialNumber) == FTDI.FT_STATUS.FT_OK)
            {
                IsOpened = true;
                Opened?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            uint numBytesRead = 0;

            if (count > 0)
            {
                _ftdi.Read(buffer, (uint)count, ref numBytesRead);
            }

            return (int)numBytesRead;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            uint numBytesWritten = 0;
            _ftdi?.Write(buffer, count, ref numBytesWritten);
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
