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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ArduinoControlApp.Converters
{
    internal class InputDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ConvertStringToValidHexString(value as string);
            }
            catch (Exception ex)
            {
                Logger.Log.Err(ex);
            }

            return DependencyProperty.UnsetValue;
        }

        public static string ConvertStringToValidHexString(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return string.Empty;
            }

            char[] valid = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', };

            var sb = new StringBuilder();
            inputString = inputString.ToUpperInvariant().Replace(" ", "");

            int counter = 0;
            for (int i = 0; i < inputString.Length; i++)
            {
                if (valid.Contains(inputString[i]))
                {
                    sb.Append(inputString[i]);
                    counter++;
                }
                else
                {
                    return sb.ToString();
                }

                if (counter == 2)
                {
                    sb.Append(' ');
                    counter = 0;
                }
            }

            return sb.ToString();
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            string[] splited = hexString.Split(new string[] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries);

            var res = new List<byte>();

            foreach (string s in splited)
            {
                if (byte.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte val))
                {
                    res.Add(val);
                }
            }

            return res.ToArray();
        }
    }
}
