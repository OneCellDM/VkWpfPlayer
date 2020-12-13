using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VkWpfPlayer
{
    partial class TeamplatesDictonary
    {
        private void ImageBrush_ImageFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
           
        }

        private void BitmapImage_DownloadFailed(object sender, System.Windows.Media.ExceptionEventArgs e)
        {
            
            //BitmapImage  im=sender as BitmapImage;
            //im.UriSource = new Uri("Resources/MusicIcon.jpg");
        }
    }
}
