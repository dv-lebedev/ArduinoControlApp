using ArduinoControlApp.Commands;
using ArduinoControlApp.Models;
using ArduinoControlApp.Monitor;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace ArduinoControlApp.ViewModels
{
    internal class FtdiDeviceControlViewModel : BaseDeviceControlViewModel
    {
        private readonly FtdiDeviceModel _ftdiDeviceModel = new FtdiDeviceModel();

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
