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
using System.ComponentModel;

namespace ArduinoControlApp.ViewModels
{
    internal class ServoViewModel : INotifyPropertyChanged
    {
        int _prog;
        
        public int Prog
        {
            get => _prog;
            set
            {
                if (_prog != value)
                {
                    _prog = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Prog)));

                    try
                    {
                        DeviceModel.CurrentDeviceModel?.Write(0xCB, new byte[] { 0x00, (byte)_prog, });
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Err(ex);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
