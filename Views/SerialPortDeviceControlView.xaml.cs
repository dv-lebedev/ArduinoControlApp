using ComPortApp.ViewModels;
using System.Windows.Controls;

namespace ComPortApp.Views
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
