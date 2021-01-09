using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer.Pages
{
    /// <summary>
    /// Логика взаимодействия для SelectPlaylistPage.xaml
    /// </summary>
    public partial class SelectPlaylistPage : Page
    {
        public ObservableCollection<AlbumModel> AlbumsCollection= new System.Collections.ObjectModel.ObservableCollection<AlbumModel>();
        public delegate void AlbumSelected(AlbumModel albumModel);
        public event AlbumSelected AlbumSelectedEvent;
        public SelectPlaylistPage()
        {
            InitializeComponent();
            AudioListView.ItemsSource = AlbumsCollection;
            
            Loading();
        }
        public void Loading()
        {
            Tools.UI.HideElements(ErrorDialog);
            Tools.UI.ShowElements(LoadingComponent);
            var s = Tools.VkApi.Audio.GetPlaylistsAsync(ownerId: (long)Tools.VkApi.UserId, count: 200);

            s.GetAwaiter().OnCompleted(() =>
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    try
                    {
                        Tools.AddDataToObservationCollection(AlbumsCollection, s.GetAwaiter().GetResult());
                        Tools.UI.HideElements(LoadingComponent);
                        
                    }
                    catch (Exception ex)
                    {
                        Tools.loggingHandler.Log.Error(s.Exception.InnerException);
                        Tools.UI.ShowElements(ErrorDialog);

                    }
                }));
            });
        }

        private void AudioListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            
        }

        private void AudioListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AudioListView.SelectedItems.Count != 0)
            {
                AlbumSelectedEvent?.Invoke(AlbumsCollection[AudioListView.SelectedIndex]);
            }
        }

        private void ErrorDialog_Accepted()
        {
            Loading();

        }
    }
}
