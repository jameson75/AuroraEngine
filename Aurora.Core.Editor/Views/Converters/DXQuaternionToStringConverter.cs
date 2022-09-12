using SharpDX;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Aurora.Core.Editor.Views.Converters
{
    [ValueConversion(typeof(Quaternion), typeof(string))]
    public class DXQuaternionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return string.Join(",", ((Quaternion)value).ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var values = value.ToString()
                                  .Split(',')
                                  .Select(float.Parse)
                                  .ToArray();

                return new Quaternion(values);
            }
            catch
            {
                return null;
            }
        }
    }
}
