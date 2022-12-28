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
