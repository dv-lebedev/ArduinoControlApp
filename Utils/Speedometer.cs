using System.Diagnostics;

namespace ComPortApp.Monitor
{
    internal class Speedometer
    {
        private readonly int speedCalcInterval = 1000;
        private readonly Stopwatch speedTime = Stopwatch.StartNew();
        private int speedBuf = 0;

        public Speedometer()
        {

        }

        public bool TryCalculateSpeed(int bytesReceived, out double? speed)
        {
            speed = null;

            speedBuf += bytesReceived;

            if (speedTime.ElapsedMilliseconds > speedCalcInterval)
            {
                speed = speedBuf / speedTime.Elapsed.TotalSeconds;
                speedBuf = 0;
                speedTime.Restart();
                return true;
            }

            return false;
        }
    }
}
