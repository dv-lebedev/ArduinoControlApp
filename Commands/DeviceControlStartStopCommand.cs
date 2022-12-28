using ArduinoControlApp.ViewModels;
using System;
using System.Windows.Input;

namespace ArduinoControlApp.Commands
{
    class DeviceControlStartStopCommand : ICommand
    {
        private BaseDeviceControlViewModel _vm;

        public event EventHandler CanExecuteChanged;

        public DeviceControlStartStopCommand(BaseDeviceControlViewModel vm)
        {
            _vm = vm ?? throw new ArgumentNullException(nameof(vm));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                if (!_vm.Enabled)
                {
                    _vm.Start();
                }
                else
                {
                    _vm.Stop();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex, true);
            }
        }
    }
}
