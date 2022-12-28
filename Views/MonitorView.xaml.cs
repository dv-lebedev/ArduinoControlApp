/*
Copyright(c) 2022-2023 Denis Lebedev
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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

        void MonitorView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
