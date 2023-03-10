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

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ArduinoControlApp
{
    internal class Logger
    {
        const string LOG_FILE = "log.txt";
        const string DT_FMT = "yyyyMMdd_HHmmss.fff";

        readonly StreamWriter _sw;

        public static readonly Logger Log = new Logger();

        Logger()
        {
            _sw = new StreamWriter(LOG_FILE, true);
        }

        ~Logger()
        {

        }

        public void Msg(string message)
        {
            _sw.WriteLine(DateTime.Now.ToString(DT_FMT));
            _sw?.WriteLine(message);
            _sw?.WriteLine(Environment.NewLine);
        }

        public void Err(Exception ex, bool notifyUser = false)
        {
            Msg(ex.ToString());

            Debug.WriteLine(ex.ToString());

            if (notifyUser)
            {
                MessageBox.Show(ex.Message, "ArduinoControlApp");
            }
        }
    }
}
