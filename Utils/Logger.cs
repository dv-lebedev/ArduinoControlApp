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
                MessageBox.Show(ex.Message);
            }
        }
    }
}
