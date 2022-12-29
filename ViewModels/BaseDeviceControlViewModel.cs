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

using ArduinoControlApp.Commands;
using ArduinoControlApp.Models;
using System;
using System.Windows.Threading;

namespace ArduinoControlApp.ViewModels
{
    internal class BaseDeviceControlViewModel : BaseViewModel
    {
        DeviceModel                 _deviceModel;
        readonly DispatcherTimer    _timer;
        double                      _receivedSpeed;
        double                      _transmitSpeed;

        public DeviceControlStartStopCommand DeviceControlStartStopCommand { get; }

        public double ReceivedSpeed
        {
            get => _receivedSpeed;

            protected set
            {
                if (_receivedSpeed != value)
                {
                    _receivedSpeed = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double TransmitSpeed
        {
            get => _transmitSpeed;

            protected set
            {
                if (_transmitSpeed != value)
                {
                    _transmitSpeed = value;
                    RaisePropertyChanged();
                }
            }
        }

        public DeviceModel DeviceModel
        {
            get => _deviceModel;

            protected set
            {
                if (_deviceModel != value)
                {
                    _deviceModel = value;
                }

                RaisePropertyChanged();
            }
        }

        public BaseDeviceControlViewModel()
        {
            try
            {
                DeviceControlStartStopCommand = new DeviceControlStartStopCommand(this);

                _timer = new DispatcherTimer(
                    new TimeSpan(0, 0, 1),
                    DispatcherPriority.Normal,
                    (s, e) => UpdateUI(),
                    System.Windows.Application.Current.Dispatcher);
                _timer.Start();
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex, true);
            }
        }

        protected virtual void UpdateUI()
        {
            try
            {
                ReceivedSpeed = DeviceModel?.ReceivedSpeed ?? 0;
                TransmitSpeed = DeviceModel?.TransmitSpeed ?? 0;
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }
        }

        public void Start()
        {
            DeviceModel?.Connect();
        }

        public void Stop()
        {
            DeviceModel?.Disconnect();
        }
    }
}
