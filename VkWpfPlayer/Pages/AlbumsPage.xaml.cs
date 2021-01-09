using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using VkWpfPlayer.DataModels;

namespace VkWpfPlayer.Pages
{
    public partial class AlbumsPage : Page
    {
        private Thread _currentThread;
        private bool d = false;

        public ObservableCollection<AlbumModel> AlbumsCollection = new ObservableCollection<AlbumModel>()
        {
        };

        public ObservableCollection<AudioModel> AudioCollection = new ObservableCollection<AudioModel>();

        public AlbumsPage()
        {
            InitializeComponent();

            AlbumsListView.Items.Clear();
            AlbumsListView.ItemsSource = AlbumsCollection;
            AudioListView.ItemsSource = AudioCollection;

            Tools.UI.HideElements(ErrorDialog, AlbumGrid);
            Tools.UI.ShowElements(LoadingComponent);
            Task.Run(() => LoadAlbums());
        }

        public void LoadAudioFromAlbum(long album_id, long owner_id)
        {
            Tools.UI.HideElements(AlbumsListView);
            Tools.UI.ShowElements(LoadingComponent, AlbumGrid);
            var DataAwaiter = Tools.VkApi.Audio.GetAsync(new VkNet.Model.RequestParams.AudioGetParams()
            {
                Count = 6000,
                PlaylistId = album_id,
                OwnerId = owner_id,
            }).GetAwaiter();
            DataAwaiter.OnCompleted(() =>
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    try
                    {
                        Tools.AddDataToObservationCollection(AudioCollection, DataAwaiter.GetResult());
                        if (!d)
                        {
                            d = true;
                            AudioCollection.Clear();
                            Thread.Sleep(200);
                            Tools.AddDataToObservationCollection(AudioCollection, DataAwaiter.GetResult());
                        }
                        Tools.UI.HideElements(LoadingComponent);
                    }
                    catch (Exception EX)
                    {
                        Tools.loggingHandler.Log.Error(EX);
                        _currentThread = new Thread(() => { LoadAudioFromAlbum(album_id, owner_id); });
                        Tools.UI.ShowElements(ErrorDialog);
                    }
                }));
            });
        }

        public void LoadAlbums()
        {
            
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
                       _currentThread = new Thread(() => { LoadAlbums(); });
                       Tools.UI.ShowElements(ErrorDialog);
                   }
               }));
           });
        }

        private void AlbumsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlbumsListView.SelectedIndex != -1)
            {
                d = false;
                var album = (AlbumModel)e.AddedItems[e.AddedItems.Count - 1];
                LoadAudioFromAlbum(album.ID, album.OwnerID);

                AlbumTitleText.Text = album.Title;

                AlbumsListView.SelectedIndex = -1;
            }
        }

        private void BackAlbumListButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                AudioCollection.Clear();
                Tools.UI.HideElements(AlbumGrid);
                Tools.UI.ShowElements(AudioListView); ;
            }));
        }

        private void AlbumsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void AudioListView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) =>

            AudioListView.SelectedAudioPlay();

        private void ErrorDialog_Accepted()
        {
            this.ErrorDialog.Visibility = Visibility.Collapsed;
            _currentThread.Start();
        }
    }
}