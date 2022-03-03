using System;
using System.Globalization;
using ToDoApp.Helpers;
using Xamarin.Forms;

namespace ToDoApp.Converters
{
    public class TaskStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isDone = (bool) value;
            return isDone ? FontAwesomeIcons.CheckCircle : FontAwesomeIcons.Circle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}