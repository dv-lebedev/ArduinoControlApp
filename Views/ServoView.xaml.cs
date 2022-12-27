using ComPortApp.ViewModels;
using System.Windows.Controls;

namespace ComPortApp.Views
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
