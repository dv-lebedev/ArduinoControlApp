using ArduinoControlApp.ViewModels;
using System.Windows.Controls;

namespace ArduinoControlApp.Views
{
    public partial class FtdiDeviceControlView : UserControl
    {
        public FtdiDeviceControlView()
        {
            InitializeComponent();
            DataContext = new FtdiDeviceControlViewModel { };
        }
    }
}
