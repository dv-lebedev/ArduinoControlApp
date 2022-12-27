using ArduinoDecoder;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ComPortApp.Monitor
{
    public class StatisticItem : INotifyPropertyChanged
    {
        private bool _checked;
        private byte _address;
        private long _count;
        private long _crcHeaderErr;
        private long _crcOverallErr;

        public bool Checked
        {
            get => _checked; 
            set => _checked = value;
        }

        public byte Address
        {
            get => _address;
            set
            {
                _address = value;
            }
        }

        public long Count
        {
            get => _count;
            set
            {
                _count = value;
            }
        }

        public long CrcHeaderErr
        {
            get => _crcHeaderErr;
            set 
            { 
                _crcHeaderErr = value;
            }
        }

        public long CrcOverallErr
        {
            get => _crcOverallErr;
            set
            {
                _crcOverallErr = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }

    public class Statistics
    {
        private readonly ConcurrentDictionary<byte, StatisticItem> _internalItems;

        public ObservableCollection<StatisticItem> Items { get; }

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
