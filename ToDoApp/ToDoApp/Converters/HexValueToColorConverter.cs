using System;
using System.Globalization;
using Xamarin.Forms;

namespace ToDoApp.Converters
{
    public class HexValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hexValue = value as string;
            if (hexValue is null)
            {
                return Color.Default;
            }

            //return ColorConverters.FromHex(hexValue);
            return Color.FromHex(hexValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}