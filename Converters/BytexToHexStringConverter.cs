using ComPortApp;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ArduinoControlApp.Converters
{
    internal class BytexToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is byte[] data && data.Length > 0)
                {
                    if (data.Length <= 10)
                    {
                        return BitConverter.ToString(data, 0);
                    }
                    else
                    {
                        return BitConverter.ToString(data, 0, 10) + " ... ";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
