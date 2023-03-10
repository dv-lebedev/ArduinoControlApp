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

using ArduinoControlApp.Coder;
using ArduinoControlApp.Commands;
using ArduinoControlApp.Converters;
using ArduinoControlApp.Entities;
using ArduinoControlApp.Interfaces;
using ArduinoControlApp.Models;
using ArduinoControlApp.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ArduinoControlApp.ViewModels
{
    internal class MonitorViewModel : INotifyPropertyChanged, IDataConsumer
    {
        readonly DispatcherTimer    _timer;
        bool                        _enabled;
        byte                        _selectedAddress;
        byte[]                      _inputData;
        string                      _inputAsString;
        readonly object             _locker = new object();


        readonly ConcurrentQueue<Package>                   _recentPackagesTemp;
        readonly ConcurrentQueue<ProtocolErrorEventArgs>    _protocolErrorsTemp;
        

        public event PropertyChangedEventHandler    PropertyChanged;
        public ObservableCollection<byte>           Addresses { get; }
        public SendPackageCommand                   SendPackageCommand { get; }
        public Statistics                           Stats { get; }
        public ObservableRangeCollection<Package>   RecentPackages { get; }
        public ObservableRangeCollection<ProtocolErrorEventArgs> ProtocolErrors { get; }
        public ICommand ClearCommand { get; }

        public bool Enabled
        {
            get => _enabled;
            private set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Enabled)));
                }
            }
        }

        public byte SelectedAddress
        {
            get => _selectedAddress;
            set
            {
                if (_selectedAddress != value)
                {
                    _selectedAddress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAddress)));
                }
            }
        }

        public byte[] InputData
        {
            get => _inputData;
            set
            {
                if (_inputData != value)
                {
                    _inputData = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputData)));
                }
            }
        }

        public string InputDataAsString
        {
            get => _inputAsString;
            set
            {
                if (_inputAsString != value)
                {
                    _inputAsString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputDataAsString)));
                }
            }
        }

        public MonitorViewModel()
        {
            try
            {
                Addresses = new ObservableCollection<byte>();

                SendPackageCommand = new SendPackageCommand(this);
                Stats = new Statistics();
                RecentPackages = new ObservableRangeCollection<Package>();
                ProtocolErrors = new ObservableRangeCollection<ProtocolErrorEventArgs>();

                _recentPackagesTemp = new ConcurrentQueue<Package>();
                _protocolErrorsTemp = new ConcurrentQueue<ProtocolErrorEventArgs>();

                BindingOperations.EnableCollectionSynchronization(RecentPackages, this);

                for (int i = 0; i < 256; i++)
                {
                    Addresses.Add((byte)i);
                }

                SelectedAddress = Addresses?.FirstOrDefault() ?? 0;

                DeviceModel.CurrentDeviceModelChanged += (__, ___) =>
                {
                    if (DeviceModel.CurrentDeviceModel != null)
                    {
                        DeviceModel.CurrentDeviceModel.DataConsumer = this;
                        DeviceModel.CurrentDeviceModel.Disconnected += (s, _) => Enabled = false;
                    }
                };

                ClearCommand = new MonitorClearCommand(this);

                _timer = new DispatcherTimer(
                    new TimeSpan(0, 0, 0, 0, 500),
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

        private void UpdateUI()
        {
            try
            {
                // update data
                int count = _recentPackagesTemp.Count;

                while (count > 0)
                {
                    if (_recentPackagesTemp.TryDequeue(out Package package))
                    {
                        if (Stats.SelectedAddrs.Count == 0)
                        {
                            RecentPackages.Add(package);
                        }
                        else
                        {
                            if (Stats.SelectedAddrs.Contains(package.Addr))
                            {
                                RecentPackages.Add(package);
                            }
                        }

                        Stats.Update(package);
                    }

                    count--;
                }

                if (RecentPackages.Count > 10_000)
                {
                    RecentPackages.RemoveFirst(RecentPackages.Count / 4);
                }

                //update errors
                count = _protocolErrorsTemp.Count;

                if (count > 0)
                {
                    while (count > 0)
                    {
                        if (_protocolErrorsTemp.TryDequeue(out ProtocolErrorEventArgs err))
                        {
                            App.Current.Dispatcher.InvokeAsync(() => ProtocolErrors.Add(err));
                        }

                        count--;
                    }
                }

                if (ProtocolErrors.Count > 10_000)
                {
                    ProtocolErrors.RemoveFirst(ProtocolErrors.Count / 4);
                }

                App.Current.Dispatcher.InvokeAsync(Stats.Refresh);
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }
        }

        public void Consume(Package package)
        {
            if (package != null)
            {
                _recentPackagesTemp.Enqueue(package);
            }
        }

        public void ProcessError(ProtocolErrorEventArgs err)
        {
            if (err != null)
            {
                _protocolErrorsTemp.Enqueue(err);
            }
        }

        internal void SendInputData()
        {
            if (InputDataAsString?.Length > 0)
            {
                InputData = InputDataConverter.ConvertHexStringToByteArray(InputDataAsString);

                DeviceModel.CurrentDeviceModel?.Write(SelectedAddress, InputData);
            }
        }
    }
}
