using System;

namespace ArduinoControlApp.Entities
{
    public class ProtocolErrorEventArgs : EventArgs
    {
        public DateTime Timestamp { get; set; }
        public ulong Offset { get; set; }
        public string ErrorType { get; set; }
    }
}
