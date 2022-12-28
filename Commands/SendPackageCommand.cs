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

using ArduinoControlApp.ViewModels;
using System;
using System.Windows.Input;

namespace ArduinoControlApp.Commands
{
    class SendPackageCommand : ICommand
    {
        readonly MonitorViewModel _monitorViewModel;

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
