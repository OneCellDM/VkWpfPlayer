using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace VkWpfPlayer
{
  public  class ImageConverter : IValueConverter
    {
        BitmapImage image;
        String FileNamePath;
        String AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\VKM";
        public Object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine(value);
            

                if (value != null)
                {
                    string[] sp = value.ToString().Split('/');
                    FileNamePath = sp[sp.Length - 2] + "_" + sp[sp.Length - 1].Split('?')[0];
                    FileNamePath = AppData + "\\" + FileNamePath;
                }
            Console.WriteLine(value.ToString());
            return 0;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
