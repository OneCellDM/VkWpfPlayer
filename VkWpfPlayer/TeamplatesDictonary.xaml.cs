using System;
using System.Diagnostics;
using System.Windows.Controls;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer
{
    partial class TeamplatesDictonary
    {
        
        public delegate void AudioSender(AudioModel audioModel, object sender);
        public delegate void Audio(AudioModel audioModel);
        public delegate void ArtistClick(String Artist);
        public static event ArtistClick ArtistClicked;
        public static event Audio AudioDelete;
        public static event Audio AudioAddToPlaylist;
        public static event AudioSender AudioAdd;

        public static event AudioSender AudioDownload;
        private void TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            ArtistClicked?.Invoke(((TextBlock)sender).Text);
            e.Handled = true;
        }
        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e) =>
            AudioDelete?.Invoke((AudioModel)((Button)sender).DataContext);


        private void AddAudioButton_Click(object sender, System.Windows.RoutedEventArgs e) =>
            AudioAdd?.Invoke((AudioModel)((Button)sender).DataContext, sender);
        private void DownloadButton_Click(object sender, System.Windows.RoutedEventArgs e)=>
            AudioDownload?.Invoke((AudioModel)((Button)sender).DataContext,sender);
        


        private void AddToplaylistButton_Click(object sender, System.Windows.RoutedEventArgs e) 
            {
           
            AudioAddToPlaylist?.Invoke((AudioModel)((Button) sender).DataContext);
            }

        
    }
}
