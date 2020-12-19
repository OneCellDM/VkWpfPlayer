using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        private void Border_Initialized(object sender, EventArgs e)
        {

        }



        private void Img_Initialized(object sender, EventArgs e)
        {
           
            

        }

        private void Img_SourceUpdated_1(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            
        }

        private void Img_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
           
            
        }
    }
}
