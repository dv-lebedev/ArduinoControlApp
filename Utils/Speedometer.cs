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

using System.Diagnostics;

namespace ArduinoControlApp.Utils
{
    internal class Speedometer
    {
        readonly int            _speedCalcInterval = 1000;
        readonly Stopwatch      _speedTime = Stopwatch.StartNew();
        int                     _speedBuf = 0;

        public Speedometer()
        {
        }

        public bool TryCalculateSpeed(int bytesReceived, out double? speed)
        {
            speed = null;

            _speedBuf += bytesReceived;

            if (_speedTime.ElapsedMilliseconds > _speedCalcInterval)
            {
                speed = _speedBuf / _speedTime.Elapsed.TotalSeconds;
                _speedBuf = 0;
                _speedTime.Restart();
                return true;
            }

            return false;
        }
    }
}
