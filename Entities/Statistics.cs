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


using ArduinoControlApp.Coder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ArduinoControlApp.Entities
{
    public class Statistics
    {
        readonly object                                     _lock = new object();           
        readonly ConcurrentDictionary<byte, StatisticItem>  _internalItems;
        readonly List<byte>                                 _selectedAddrs = new List<byte> { };

        public ObservableCollection<StatisticItem> Items { get; }
        public List<byte> SelectedAddrs { get { return _selectedAddrs; } }

        public Statistics()
        {
            _internalItems = new ConcurrentDictionary<byte, StatisticItem>();
            Items = new ObservableCollection<StatisticItem>();
        }

        public void Update(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (!_internalItems.TryGetValue(package.Addr, out var item))
            {
                item = new StatisticItem
                {
                    Address = package.Addr,
                };

                item.CheckedEvent += (s, e) =>
                {
                    if (item.Checked)
                    {
                        _selectedAddrs.Add(item.Address);
                    }
                    else
                    {
                        _selectedAddrs.Remove(item.Address);
                    }
                };

                App.Current?.Dispatcher?.Invoke(() =>
                {
                    if (_internalItems.TryAdd(package.Addr, item))
                    {
                            Items.Add(item);
                        
                    }
                });
            }

            item.Count++;
            item.CrcHeaderErr += package.CrcHeaderErr ? 1 : 0;
            item.CrcOverallErr += package.OverallCrcErr ? 1 : 0;
        }

        public void Clear()
        {
            _internalItems.Clear();

            try
            {
                Items.Clear();
                System.Threading.Thread.Sleep(150);
            }
            catch (InvalidOperationException ex)
            {
                Logger.Log.Err(ex);
            }
        }

        public void Refresh()
        {
            App.Current?.Dispatcher?.Invoke(() =>
            {
                foreach (var item in _internalItems.Values)
                {
                    item.NotifyPropertyChanged();
                }
            });
        }
    }
}
