using ComPortApp.ViewModels;
using System.Windows.Controls;

namespace ComPortApp.Views
{
    public partial class ToneGenView : UserControl
    {
        public ToneGenView()
        {
            InitializeComponent();
            DataContext = new ToneGenViewModel { };
        }
    }
}
