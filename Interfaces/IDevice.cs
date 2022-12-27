using System;

namespace ComPortApp.Monitor
{
    public interface IDevice : IDisposable
    {
        void Open();
        void Close();
        int Read(byte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);
    }

}
