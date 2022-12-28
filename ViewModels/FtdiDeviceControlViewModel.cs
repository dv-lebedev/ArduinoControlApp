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

using ArduinoControlApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ArduinoControlApp.ViewModels
{
    internal class FtdiDeviceControlViewModel : BaseDeviceControlViewModel
    {
        readonly FtdiDeviceModel _ftdiDeviceModel = new FtdiDeviceModel();

        public ObservableCollection<string> SerialNumbers { get; private set; } = new ObservableCollection<string>();

        public string SelectedSerialNumber
        {
            get => _ftdiDeviceModel.SerialNumber;

            set
            {
                _ftdiDeviceModel.SerialNumber = value ?? throw new ArgumentException(nameof(SelectedSerialNumber));
                RaisePropertyChanged();
            }
        }

        public FtdiDeviceControlViewModel()
        {
            DeviceModel = _ftdiDeviceModel;
        }

        protected override void UpdateUI()
        {
            try
            {
                base.UpdateUI();

                if (!Enabled)
                {
                    RefreshSerialNumbersList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }
        }

        private void RefreshSerialNumbersList()
        {
            string[] available = _ftdiDeviceModel.GetAvailableSerialNumbers();

            if ((available == null || available.Length == 0) && SerialNumbers.Count > 0)
            {
                SerialNumbers.Clear();
            }

            foreach (var ex in SerialNumbers.ToList())
            {
                if (!available.Contains(ex))
                {
                    SerialNumbers.Remove(ex);
                }
            }

            foreach (var fresh in available)
            {
                if (!SerialNumbers.Contains(fresh))
                {
                    SerialNumbers.Add(fresh);
                }
            }
        }
    }
}
