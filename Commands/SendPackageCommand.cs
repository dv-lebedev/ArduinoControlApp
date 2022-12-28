using ArduinoControlApp.ViewModels;
using System;
using System.Windows.Input;

namespace ArduinoControlApp.Commands
{
    class SendPackageCommand : ICommand
    {
        private MonitorViewModel _monitorViewModel;

        public event EventHandler CanExecuteChanged;

        public SendPackageCommand(MonitorViewModel monitorViewModel)
        {
            _monitorViewModel = monitorViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                _monitorViewModel?.SendInputData();
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex, true);
            }
        }
    }

}
