using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Aurora.Core.Editor.Views.Converters
{
    [ValueConversion(typeof(SharpDX.Color), typeof(Color))]
    public class DXColorToMediaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sharpDXColor = (SharpDX.Color)value;
            return Color.FromArgb(sharpDXColor.A, sharpDXColor.R, sharpDXColor.G, sharpDXColor.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var color = (Color)value;
                return new SharpDX.Color(color.R, color.G, color.B, color.A);
            }
            catch
            {
                return null;
            }
        }
    }
}

