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
using ArduinoControlApp.Entities;
using ArduinoControlApp.Interfaces;
using ArduinoControlApp.Utils;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ArduinoControlApp.Models
{
    public abstract class DeviceModel : INotifyPropertyChanged
    {
        readonly ConcurrentQueue<byte[]>    _transmitQueue = new ConcurrentQueue<byte[]>();
        readonly Speedometer                _recSpeed = new Speedometer();
        readonly Speedometer                _transSpeed = new Speedometer();
        IDevice                             _currentDevice;
        CancellationTokenSource             _recCts;
        CancellationTokenSource             _transCts;
        double                              _receivedSpeed;
        double                              _transmitSpeed;
        bool                                _isConnected;
        static DeviceModel                  _currentDeviceModel;

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public static event EventHandler CurrentDeviceModelChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public RCoder                       Coder { get; } = new RCoder();
        public IDataConsumer                DataConsumer { get; set; }

        public IDevice CurrentDevice
        {
            get => _currentDevice;

            protected set
            {
                _currentDevice = value;
            }
        }

        public double ReceivedSpeed
        {
            get => _receivedSpeed;

            private set
            {
                if (_receivedSpeed != value)
                {
                    _receivedSpeed = value;
                }
            }
        }

        public double TransmitSpeed
        {
            get => _transmitSpeed;

            private set
            {
                if (_transmitSpeed != value)
                {
                    _transmitSpeed = value;
                }
            }
        }

        public bool IsConnected
        {
            get => _isConnected;

            protected set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));
                }
            }
        }

        public static DeviceModel CurrentDeviceModel
        {
            get => _currentDeviceModel;

            set
            {
                if (_currentDeviceModel != value)
                {
                    _currentDeviceModel = value;
                    CurrentDeviceModelChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public DeviceModel() 
        {
            if (Coder != null)
            {
                Coder.OnPackageReceived += Coder_OnPackageReceived;
                Coder.GotError += Coder_GotError;
            }
        }

        private void Coder_GotError(object sender, ProtocolErrorEventArgs e)
        {
            DataConsumer?.ProcessError(e);
        }

        private void Coder_OnPackageReceived(object sender, Package e)
        {
            DataConsumer?.Consume(e);
        }

        protected abstract IDevice CreateDeviceBeforeConnect();

        public void Connect()
        {
            if (CurrentDeviceModel != null && CurrentDeviceModel.IsConnected)
            {
                CurrentDeviceModel.Disconnect();
            }

            _currentDevice = CreateDeviceBeforeConnect();
            
            if (_currentDevice != null)
            {
                CurrentDeviceModel = this;

                _currentDevice.Opened += (s, e) =>
                {
                    IsConnected = true;

                    _recCts = new CancellationTokenSource();
                    Task.Run(() => Receiver(_recCts.Token), _recCts.Token);

                    _transCts = new CancellationTokenSource();
                    Task.Run(() => Transmitter(_transCts.Token), _transCts.Token);

                    Connected?.Invoke(this, EventArgs.Empty);
                };

                _currentDevice.Open();             
            }
        }

        public void Disconnect(bool closeDevice = true)
        {
            _recCts?.Cancel();
            _transCts?.Cancel();

            if (closeDevice)
            {
                _currentDevice?.Close();
            }
        }

        public void Write(byte addr, byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length == 0) return;

            byte[] package = RCoder.CreatePackage(addr, data);

            _transmitQueue.Enqueue(package);
        }

        private void Receiver(CancellationToken ct)
        {
            try
            {
                byte[] buffer = new byte[65535 / 2];

                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    int read = _currentDevice?.Read(buffer, 0, buffer.Length) ?? 0;

                    if (read > 0)
                    {
                        Coder?.Decode(buffer, 0, read);

                        if (_recSpeed.TryCalculateSpeed(read, out double? recSpeedValue))
                        {
                            ReceivedSpeed = recSpeedValue ?? 0;
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }
            finally
            {
                ReceivedSpeed = 0;
                Disconnected?.Invoke(this, EventArgs.Empty);
                IsConnected = false;
            }
        }

        private void Transmitter(CancellationToken ct)
        {
            try
            {
                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    if (!_transmitQueue.IsEmpty)
                    {
                        int count = _transmitQueue.Count;

                        int written = 0;
                        while (count > 0)
                        {
                            if (_transmitQueue.TryDequeue(out var package))
                            {
                                _currentDevice?.Write(package, 0, package.Length);
                                count--;
                                written += package.Length;
                            }

                            if (_transSpeed.TryCalculateSpeed(written, out double? transSpeedValue))
                            {
                                TransmitSpeed = transSpeedValue ?? 0;
                            }
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }
            finally
            {
                TransmitSpeed = 0;
                //Disconnected?.Invoke(this, new EventArgs());
                IsConnected = false;
            }
        }
    }
}
