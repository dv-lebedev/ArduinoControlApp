using ComPortApp.ViewModels;
using System.Windows.Controls;

namespace ComPortApp.Views
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
