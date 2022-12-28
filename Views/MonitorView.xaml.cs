using ArduinoControlApp.ViewModels;
using System;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace ArduinoControlApp.Views
{
    public partial class MonitorView : UserControl
    {
        public MonitorView()
        {
            InitializeComponent();

            var vm = new MonitorViewModel();
            vm.RecentPackage.CollectionChanged += MonitorView_CollectionChanged;
            DataContext = vm;
        }

        private void MonitorView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                App.Current?.Dispatcher?.InvokeAsync(() =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        int lastIndex = listView.Items.Count - 1;

                        if (lastIndex < 0)
                        {
                            lastIndex = 0;
                        }

                        if (listView.Items.Count > 0)
                        {
                            listView.ScrollIntoView(listView.Items[lastIndex]);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }
        }
    }
}
