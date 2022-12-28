using ArduinoControlApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ArduinoControlApp.ViewModels
{
    internal class SerialPortDeviceControlViewModel : BaseDeviceControlViewModel
    {
        private readonly SerialPortDeviceModel _serialPortDeviceModel = new SerialPortDeviceModel();
        private string _selectedPort;
        private int _selectedSpeed = 9600;

        public ObservableCollection<string> PortsList { get; }
        public ObservableCollection<int> SpeedsList { get; }

        public string SelectedPort
        {
            get => _selectedPort;
            set
            {
                _selectedPort = value;
                RaisePropertyChanged();

                _serialPortDeviceModel.PortName = _selectedPort;
            }
        }

        public int SelectedSpeed
        {
            get => _selectedSpeed;
            set
            {
                _selectedSpeed = value;
                RaisePropertyChanged();

                _serialPortDeviceModel.Baudrate = _selectedSpeed;
            }
        }

        public SerialPortDeviceControlViewModel()
        {
            try
            {
                PortsList = new ObservableCollection<string>();
                SpeedsList = new ObservableCollection<int>();

                RefreshPortsList();

                SelectedPort = PortsList?.FirstOrDefault();

                foreach (int speed in new[] { 9600, 19200, 128000, })
                {
                    SpeedsList?.Add(speed);
                }

                SelectedSpeed = SpeedsList?.FirstOrDefault() ?? 9600;

                DeviceModel = _serialPortDeviceModel;
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex, true);
            }
        }

        protected override void UpdateUI()
        {
            try
            {
                base.UpdateUI();

                if (!Enabled)
                {
                    RefreshPortsList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }
        }

        void RefreshPortsList()
        {
            string[] portsFounded = GetAvailablePorts();

            if ((portsFounded == null ||
                portsFounded.Length == 0) &&
                PortsList.Count > 0)
            {
                PortsList.Clear();
                return;
            }

            foreach (var existed in PortsList.ToList())
            {
                if (!string.IsNullOrEmpty(existed))
                {
                    var f = portsFounded.FirstOrDefault(i => i == existed);
                    if (f == null)
                    {
                        PortsList.Remove(existed);
                    }
                }
            }

            foreach (var founded in portsFounded)
            {
                if (!string.IsNullOrEmpty(founded))
                {
                    var existed = PortsList.FirstOrDefault(i => i == founded);
                    if (string.IsNullOrEmpty(existed))
                    {
                        PortsList.Add(founded);
                    }
                }
            }

            if (SelectedPort == null)
            {
                SelectedPort = PortsList.FirstOrDefault();
            }
        }

        string[] GetAvailablePorts()
        {
            try
            {
                return _serialPortDeviceModel?.GetAvailablePorts() ?? Array.Empty<string>();
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }

            return Array.Empty<string>();
        }
    }
}
