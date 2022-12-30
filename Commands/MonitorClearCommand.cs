using ArduinoControlApp.ViewModels;
using System;
using System.Windows.Input;

namespace ArduinoControlApp.Commands
{
    internal class MonitorClearCommand : ICommand
    {
        private readonly MonitorViewModel _vm;

        public MonitorClearCommand(MonitorViewModel monitorViewModel)
        {
            _vm = monitorViewModel ?? throw new ArgumentNullException(nameof(monitorViewModel));
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _vm.Stats?.Clear();
            _vm.RecentPackages?.Clear();
            _vm.ProtocolErrors?.Clear();
        }
    }
}
