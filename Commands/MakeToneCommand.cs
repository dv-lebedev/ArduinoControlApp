using ComPortApp.Monitor;
using System;
using System.Windows.Input;

namespace ComPortApp.Commands
{
    internal class MakeToneCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                int duration = 2000;

                if (parameter is int freq)
                {
                    byte[] data = new byte[8];

                    data[0] = (byte)(freq >> 24);
                    data[1] = (byte)(freq >> 16);
                    data[2] = (byte)(freq >> 8);
                    data[3] = (byte)freq;

                    data[4] = (byte)(duration >> 24);
                    data[5] = (byte)(duration >> 16);
                    data[6] = (byte)(duration >> 8); 
                    data[7] = (byte)duration;

                    DeviceModel.CurrentDeviceModel?.Write(0xAA, data);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex, true);
            }
        }
    }
}
