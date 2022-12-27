using ComPortApp.Commands;
using ComPortApp.Monitor;
using System;
using System.Windows.Threading;

namespace ComPortApp.ViewModels
{
    internal class BaseDeviceControlViewModel : BaseViewModel
    {
        private DeviceModel _deviceModel;
        private readonly DispatcherTimer _timer;
        private bool _enabled;
        private double _receivedSpeed;
        private double _transmitSpeed;

        public DeviceControlStartStopCommand DeviceControlStartStopCommand { get; }

        public bool Enabled
        {
            get => _enabled;
            private set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double ReceivedSpeed
        {
            get => _receivedSpeed;
            set
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
            set
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
            set
            {
                if (_deviceModel != value)
                {
                    _deviceModel = value;

                    if (_deviceModel != null)
                    {
                        _deviceModel.Disconnected += (s, _) => Enabled = false;
                    }
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

                RaisePropertyChanged(nameof(BaseDeviceControlViewModel));
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }
        }

        internal void Start()
        {
            DeviceModel?.Connect();
            Enabled = true;
        }

        internal void Stop()
        {
            DeviceModel?.Disconnect();
            Enabled = false;
        }
    }
}
