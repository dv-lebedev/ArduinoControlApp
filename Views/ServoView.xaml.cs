using ArduinoControlApp.ViewModels;
using System.Windows.Controls;

namespace ArduinoControlApp.Views
{
    public partial class ServoView : UserControl
    {
        public ServoView()
        {
            InitializeComponent();
            DataContext = new ServoViewModel { };
        }
    }
}
