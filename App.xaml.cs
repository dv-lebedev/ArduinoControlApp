using ComPortApp.Monitor;
using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ComPortApp
{
    public partial class App : Application
    {
        private static DeviceModel _currentDeviceModel;

        public static DeviceModel CurrentDeviceModel
        {
            get => _currentDeviceModel;
            set
            {
                _currentDeviceModel = value;
                CurrentDeviceModelChanged?.Invoke(App.Current, EventArgs.Empty);
            }
        }

        public static event EventHandler CurrentDeviceModelChanged;

        public App()
        {
        }
    }
}
