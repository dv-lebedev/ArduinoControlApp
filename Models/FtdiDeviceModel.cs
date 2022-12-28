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

using ArduinoControlApp.Ftdi;
using System;

namespace ArduinoControlApp.Models
{
    internal class FtdiDeviceModel : DeviceModel
    {
        string _serialNumber;

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
