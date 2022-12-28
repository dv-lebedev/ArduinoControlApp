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

using ArduinoControlApp.Models;
using System;
using System.Windows.Input;

namespace ArduinoControlApp.Commands
{
    internal class MakeToneCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                int duration = 2000;

                if (parameter is int freq)
                {
                    byte[] data = new byte[8];

                    data[0] = (byte)(freq >> 24);
                    data[1] = (byte)(freq >> 16);
                    data[2] = (byte)(freq >> 8);
                    data[3] = (byte)freq;

                    data[4] = (byte)(duration >> 24);
                    data[5] = (byte)(duration >> 16);
                    data[6] = (byte)(duration >> 8); 
                    data[7] = (byte)duration;

                    DeviceModel.CurrentDeviceModel?.Write(0xAA, data);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex, true);
            }
        }
    }
}
