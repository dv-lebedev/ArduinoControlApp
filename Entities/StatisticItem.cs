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

using System.ComponentModel;

namespace ArduinoControlApp.Entities
{
    public class StatisticItem : INotifyPropertyChanged
    {
        bool _checked;
        byte _address;
        long _count;
        long _crcHeaderErr;
        long _crcOverallErr;

        public bool Checked
        {
            get => _checked; 
            set => _checked = value;
        }

        public byte Address
        {
            get => _address;
            set
            {
                _address = value;
            }
        }

        public long Count
        {
            get => _count;
            set
            {
                _count = value;
            }
        }

        public long CrcHeaderErr
        {
            get => _crcHeaderErr;
            set 
            { 
                _crcHeaderErr = value;
            }
        }

        public long CrcOverallErr
        {
            get => _crcOverallErr;
            set
            {
                _crcOverallErr = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
