
using System;
using System.Globalization;
using System.Windows.Data;

namespace VkWpfPlayer
{
    public class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            var seconds = (int)value % 60;
            return seconds > 9 ? ((int)value / 60).ToString() + ":" + seconds.ToString() : ((int)value / 60).ToString() + ":0" + seconds.ToString();

        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return "";
        }
    }


}
