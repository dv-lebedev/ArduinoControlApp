using System;
using System.ComponentModel;

namespace ComPortApp.ViewModels
{
    internal class ServoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _prog;
        public int Prog
        {
            get => _prog;
            set
            {
                if (_prog != value)
                {
                    _prog = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Prog)));

                    try
                    {
                        App.CurrentDeviceModel?.Write(0xCB, new byte[] { 0x00, (byte)_prog, });
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Err(ex);
                    }
                }
            }
        }
    }
}
