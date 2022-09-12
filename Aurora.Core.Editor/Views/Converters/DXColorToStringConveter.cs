using SharpDX;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Aurora.Core.Editor.Views.Converters
{
    [ValueConversion(typeof(Color), typeof(string))]
    public class DXColorToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return string.Join(",", ((Color)value).ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var values = value.ToString()
                                  .Split(',')
                                  .Select(byte.Parse)
                                  .ToArray();

                return new Color(values);
            }
            catch
            {
                return null;
            }
        }
    }
}

