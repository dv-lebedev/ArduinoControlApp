using ArduinoDecoder;
using ComPortApp.Commands;
using ComPortApp.Monitor;
using ComPortApp.Monitor.Converters;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace ComPortApp.ViewModels
{
    internal class MonitorViewModel : INotifyPropertyChanged, IDataConsumer
    {
        private readonly DispatcherTimer    _timer;
        private bool                        _enabled;
        private byte                        _selectedAddress;
        private byte[]                      _inputData;
        private string                      _inputAsString;
        private readonly object             _locker = new object();

        public event PropertyChangedEventHandler    PropertyChanged;
        public ObservableCollection<byte>           Addresses { get; }
        public SendPackageCommand                   SendPackageCommand { get; }
        public Statistics                           Stats { get; }
        public ObservableRangeCollection<Package>   RecentPackage { get; }

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
                RecentPackage = new ObservableRangeCollection<Package>();

                BindingOperations.EnableCollectionSynchronization(RecentPackage, this);

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

        private void UpdateUI()
        {
            try
            {
                lock (_locker)
                {
                    if (RecentPackage.Count > 100_000)
                    {
                        RecentPackage.RemoveFirst(10_000);
                        Debug.WriteLine("CLEAN");
                    }
                }

                App.Current.Dispatcher.Invoke(Stats.Refresh);
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
                lock (_locker)
                {
                    RecentPackage.Add(package);
                }

                Stats.Update(package);
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
