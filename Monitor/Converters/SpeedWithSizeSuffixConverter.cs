using ComPortApp.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ComPortApp.Monitor.Converters
{
    internal static class BytesSizeInfo
    {
        static readonly string[] SizeSuffixes =
                      { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value <= 0) { return "0 bytes"; }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }

    internal class SpeedWithSizeSuffixConverter : IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is BaseDeviceControlViewModel vm)
                {
                    return string.Join(
                        " ",
                        BytesSizeInfo.SizeSuffix((long)vm.ReceivedSpeed),
                        "/",
                        BytesSizeInfo.SizeSuffix((long)vm.TransmitSpeed));
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }

            return DependencyProperty.UnsetValue;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values?.Length == 2 && values[0] is double rec && values[1] is double trans)
                {
                    return string.Join(
                        " ",
                        BytesSizeInfo.SizeSuffix((long)rec),
                        "/",
                        BytesSizeInfo.SizeSuffix((long)trans));
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex); 
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
