using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace VkWpfPlayer
{
    partial class TeamplatesDictonary
    {
        public delegate void Audio(object audioModel);
        public  static event Audio AddAudioButtonClick;
        public delegate void ArtistClick(String Artist);
        public static event ArtistClick ArtistClicked;
        public static event Audio AudioDeleteClicked;

      
        private void TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ArtistClicked?.Invoke(((TextBlock)sender).Text);
            e.Handled = true;
            

        }

        

        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AudioDeleteClicked?.Invoke(((Button)sender).DataContext);
        }

        private void AddAudioButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddAudioButtonClick?.Invoke(((Button)sender).DataContext);
        }
    }
}
