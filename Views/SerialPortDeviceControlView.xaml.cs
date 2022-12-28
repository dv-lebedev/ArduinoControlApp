using ArduinoControlApp.ViewModels;
using System.Windows.Controls;

namespace ArduinoControlApp.Views
{
    public partial class SerialPortDeviceControlView : UserControl
    {
        public SerialPortDeviceControlView()
        {
            InitializeComponent();
            DataContext = new SerialPortDeviceControlViewModel { };
        }
    }
}
