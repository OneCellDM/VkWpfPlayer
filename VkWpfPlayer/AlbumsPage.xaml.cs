using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media.Animation;

using VkWpfPlayer.DataModels;

namespace VkWpfPlayer
{

    public partial class AlbumsPage : Page
    {
        private Thread _currentThread;
        private bool d = false;
        public ObservableCollection<AlbumModel> AlbumsCollection = new ObservableCollection<AlbumModel>();
        public ObservableCollection<AudioModel> AudioCollection = new ObservableCollection<AudioModel>();

        ThicknessAnimation ShowPlaylistAnimation = new ThicknessAnimation();
        ThicknessAnimation HidePlaylistAnimation = new ThicknessAnimation();

        public AlbumsPage()
        {
            InitializeComponent();
            ErrorGrid.Visibility = Visibility.Collapsed;
            AlbumsListView.Items.Clear();
            AlbumsListView.ItemsSource = AlbumsCollection;
            AudioListView.ItemsSource = AudioCollection;
            Task.Run(() => LoadAlbums());
            AlbumGrid.Visibility = Visibility.Collapsed;
            SuccesLoadPanel.Visibility = Visibility.Collapsed;

        }

        public void LoadAudioFromAlbum(long album_id, long owner_id)
        {

            var DataAwaiter = ToolsAndsettings.VkApi.Audio.GetAsync(new VkNet.Model.RequestParams.AudioGetParams()
            {

                Count = 6000,
                PlaylistId = album_id,
                OwnerId = owner_id,
            }).GetAwaiter();
            DataAwaiter.OnCompleted(() =>
            {
                //TODP:Отловить исключения
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    try
                    {
                        ToolsAndsettings.AddDataToObservationCollection(AudioCollection, DataAwaiter.GetResult());
                        if (!d)
                        {
                            d = true;
                            AudioCollection.Clear();
                            Thread.Sleep(200);
                            ToolsAndsettings.AddDataToObservationCollection(AudioCollection, DataAwaiter.GetResult());


                        }
                        SuccesLoadPanel.Visibility = Visibility.Collapsed;
                    }
                    catch (Exception EX)
                    {
                        ToolsAndsettings.loggingHandler.Log.Error(EX);
                        _currentThread = new Thread(() => { LoadAudioFromAlbum(album_id, owner_id); });
                        ErrorGrid.Visibility = Visibility.Visible;
                    }

                }));






            });


        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {

        }

        public void LoadAlbums()
        {


            var s = ToolsAndsettings.VkApi.Audio.GetPlaylistsAsync(ownerId: (long)ToolsAndsettings.VkApi.UserId, count: 200);

            s.GetAwaiter().OnCompleted(() =>
           {
               this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
               {
                   try
                   {
                       ToolsAndsettings.AddDataToObservationCollection(AlbumsCollection, s.GetAwaiter().GetResult());
                   }
                   catch (Exception ex)
                   {
                        //TODO: Закончить отлов ошибок и их логирование    
                        ToolsAndsettings.loggingHandler.Log.Error(s.Exception.InnerException);
                       _currentThread = new Thread(() => { LoadAlbums(); });
                       ErrorGrid.Visibility = Visibility.Visible;
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
                SuccesLoadPanel.Visibility = Visibility.Visible;
                AlbumGrid.Visibility = Visibility.Visible;

                ShowPlaylistAnimation.Completed += DoubleAnimation_Completed;
                ShowPlaylistAnimation.From = new Thickness(0, 0, 0, this.AlbumsListView.ActualHeight);
                ShowPlaylistAnimation.To = new Thickness(0, 0, 0, 0);
                ShowPlaylistAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
                AlbumGrid.BeginAnimation(MarginProperty, ShowPlaylistAnimation);


            }
        }

        private void BackAlbumListButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                AudioCollection.Clear();
                HidePlaylistAnimation.Completed += HidePlaylistAnimation_Completed;

                HidePlaylistAnimation.From = new Thickness(0, 0, 0, 0);
                HidePlaylistAnimation.To = new Thickness(0, 0, 0, this.AlbumsListView.ActualHeight);
                HidePlaylistAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
                AlbumGrid.BeginAnimation(MarginProperty, HidePlaylistAnimation);
            }));
        }

        private void HidePlaylistAnimation_Completed(object sender, EventArgs e)
        {
            AlbumGrid.Visibility = Visibility.Collapsed;
            AudioCollection.Clear();

        }



        private void AlbumsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void AudioListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ToolsAndsettings.SendListClickEvent(AudioCollection, AudioListView.SelectedIndex);
                Player.Play(((AudioModel)e.AddedItems[e.AddedItems.Count - 1]));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RetryRequests_Click(object sender, RoutedEventArgs e)
        {
            this.ErrorGrid.Visibility = Visibility.Collapsed;
            _currentThread.Start();

        }
    }

}
