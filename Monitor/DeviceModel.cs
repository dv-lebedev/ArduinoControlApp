using ArduinoDecoder;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ComPortApp.Monitor
{
    public abstract class DeviceModel
    {
        readonly ConcurrentQueue<byte[]>    _transmitQueue = new ConcurrentQueue<byte[]>();
        readonly Speedometer                _recSpeed = new Speedometer();
        readonly Speedometer                _transSpeed = new Speedometer();
        IDevice                             _currentDevice;
        CancellationTokenSource             _recCts;
        CancellationTokenSource             _transCts;
        double                              _receivedSpeed;
        double                              _transmitSpeed;

        public event EventHandler Connected;
        public event EventHandler Disconnected;

        public RCoder                       Coder { get; } = new RCoder();
        public IDataConsumer                DataConsumer { get; set; }

        public IDevice CurrentDevice
        {
            get => _currentDevice;
            set
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

        public DeviceModel() 
        {
            if (Coder != null)
            {
                Coder.OnPackageReceived += Coder_OnPackageReceived;
            }
        }

        private void Coder_OnPackageReceived(object sender, Package e)
        {
            DataConsumer?.Consume(e);
        }

        public string[] GetAvailable()
        {
            return ComPortDevice.GetAvailable();
        }

        protected abstract void InitCurrentDevice();

        public void Connect()
        {
            InitCurrentDevice();

            if (_currentDevice != null)
            {
                _currentDevice.Open();

                _recCts = new CancellationTokenSource();
                Task.Run(() => Receiver(_recCts.Token), _recCts.Token);

                _transCts = new CancellationTokenSource();
                Task.Run(() => Transmitter(_transCts.Token), _transCts.Token);

                Connected?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Disconnect()
        {
            _recCts?.Cancel();
            _transCts?.Cancel();
            _currentDevice?.Close();
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
            }
        }
    }
}
