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

using ArduinoControlApp.Serial;
using System;

namespace ArduinoControlApp.Models
{
    internal class SerialPortDeviceModel : DeviceModel
    {
        string  _portName;
        int     _baudRate;

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
