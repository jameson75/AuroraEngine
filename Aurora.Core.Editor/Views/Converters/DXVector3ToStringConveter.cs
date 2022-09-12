using SharpDX;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Aurora.Core.Editor.Views.Converters
{
    [ValueConversion(typeof(Vector3), typeof(string))]
    public class DXVector3ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Join(",", ((Vector3)value).ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var values = value.ToString()
                                  .Split(',')
                                  .Select(float.Parse)
                                  .ToArray();

                return new Vector3(values);
            }
            catch
            {
                return null;
            }
        }
    }
}

