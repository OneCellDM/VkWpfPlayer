using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using VkWpfPlayer.DataModels;
using VkWpfPlayer.Pages;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Button = System.Windows.Controls.Button;
using System.Windows.Media;

namespace VkWpfPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsHidden = false;
        private bool Minimized = false;
        private double OldWith = 0;
        private double OldHeight = 0;

        private MyAudios _MyAudiosPage;
        private AlbumsPage _AlbumsPage;
        private SearchPage _searchPage;
        private CurrentPlaylistPage _currentPlaylistPage;
        private RepostPage _repostPage;
        private RecomendatePage _recomendatePage;
        private PopularPage _popularPage;

        private Converters.DurationConverter durationconverter = new Converters.DurationConverter();

        public  void SendMessageToMamory()
        {
           
        }
        public MainWindow()
        {




            InitializeComponent();
           

            
            AuthFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            String AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VKM";
            if (!new System.IO.DirectoryInfo(AppData).Exists)
                new System.IO.DirectoryInfo(AppData).Create();

            AuthFrame.Content = new VkLogin().Content;
            Tools.AuthorizedAcces += ToolsAndsettings_AuthorizedAcces;
            Tools.CurrentSettings.BackGroundChanged += CurrentSettings_BackGroundChanged;
        }

        private void CurrentSettings_BackGroundChanged()
        {
            if(Tools.CurrentSettings.BackGroundImage != null)
            if(Tools.CurrentSettings.IsBackgroundImage &  Tools.CurrentSettings.BackGroundImage.Length>0)
            {
                try
                {
                        this.Background = new ImageBrush(new BitmapImage(new Uri(Tools.CurrentSettings.BackGroundImage))) { Stretch = Stretch.UniformToFill };
                        ((Grid)_repostPage.FindName("GridContainer")).Background = new ImageBrush(new BitmapImage(new Uri(Tools.CurrentSettings.BackGroundImage))) { Stretch = Stretch.UniformToFill };
                        this.Background.Opacity = Tools.CurrentSettings.BackGroundOpacity;
                        ((Grid)_repostPage.FindName("GridContainer")).Background.Opacity = Tools.CurrentSettings.BackGroundOpacity;
                        return;
                }catch(Exception eX)
                {

                }


            }
          
                this.Background = (Brush)new BrushConverter().ConvertFromString(Tools.CurrentSettings.BackGroundColor);
            ((Grid)_repostPage.FindName("GridContainer")).Background = (Brush)new BrushConverter().ConvertFromString(Tools.CurrentSettings.BackGroundColor);
           
        }

        private void TeamplatesDictonary_AudioAddToPlaylist(AudioModel audioModel)
        {
            SelectPlaylistPage selectPlaylistPage = new SelectPlaylistPage();
            selectPlaylistPage.ExitButton.Click += delegate (object sender,RoutedEventArgs e){
                AuthFrame.Visibility = Visibility.Collapsed;
                AuthFrame.Content = null;
                selectPlaylistPage = null;
            };
            selectPlaylistPage.AlbumSelectedEvent += delegate (AlbumModel AlbumModel)
            {
                AuthFrame.Visibility = Visibility.Collapsed;
                var Awaiter = Tools.VkApi.Audio.AddToPlaylistAsync(AlbumModel.OwnerID, AlbumModel.ID,
                    new string[] { audioModel.Owner_ID + "_" + audioModel.ID.ToString() + "_"  + audioModel.AccessKey });
                
            };
            AuthFrame.Content = selectPlaylistPage.Content;
            AuthFrame.Visibility = Visibility.Visible;

        }

        private void TeamplatesDictonary_AudioAdd(AudioModel audioModel, object sender)
        {
            var Awaiter = Tools.VkApi.Audio.AddAsync(audioModel.ID, audioModel.Owner_ID, audioModel.AccessKey).GetAwaiter();
            Awaiter.OnCompleted(() =>
            {
                if (Awaiter.GetResult() != 0)
                {
                    ((Button)sender).Content = System.Net.WebUtility.HtmlDecode("&#xE73E;");
                    var newModel = new AudioModel()
                    {
                        AccessKey = audioModel.AccessKey,
                        Artist = audioModel.Artist,
                        AudioUrl = audioModel.AudioUrl,
                        ImageUrl = audioModel.ImageUrl,
                        Title = audioModel.Title,
                        DurationSeconds = audioModel.DurationSeconds,
                        Owner_ID = (long)Tools.VkApi.UserId,
                        ID = Awaiter.GetResult(),
                    };
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                    {
                        Tools.AddDataToObservationCollection(_MyAudiosPage.AudioCollection, newModel);
                        _MyAudiosPage.AudioCollection.Move(_MyAudiosPage.AudioCollection.Count - 1, 0);
                    }));
                }
            });
        }

        private void TeamplatesDictonary_AudioDeleteClicked(AudioModel audioModel)
        {
            var taskAwaiter = Tools.VkApi.Audio.DeleteAsync(audioModel.ID, audioModel.Owner_ID).GetAwaiter();
            taskAwaiter.OnCompleted(() =>
            {
                var taskAwaiter2 = Tools.VkApi.Audio.GetByIdAsync(new String[]
                { audioModel.ID.ToString() + "_" + audioModel.Owner_ID.ToString()}).GetAwaiter();
                taskAwaiter2.OnCompleted(() =>
                {
                    try
                    {
                        var res = taskAwaiter2.GetResult();
                    }
                    catch (VkNet.Exception.ParameterMissingOrInvalidException ex)
                    {
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                        {
                            if (MyAudiosTab.IsSelected)
                                _MyAudiosPage.AudioCollection.Remove(audioModel);

                            if (MyPlayliststab.IsSelected)
                                _AlbumsPage.AudioCollection.Remove(audioModel);
                            
                        }));
                    }
                });
            });
        }

        private void TeamplatesDictonary_ArtistClicked(string Artist)
        {
            _searchPage.SearchTextBox.Text = Artist;

            _searchPage.AllAudioFound = false;
            _searchPage.offset = 0;
            _searchPage.SearchCollection.Clear();
            _searchPage.LoadAudios();
            SearchTab.IsSelected = true;
        }

        private void ToolsAndsettings_AuthorizedAcces()
        {
            Player.UpdatePosition += Player_UpdatePosition;
            Player.updateAudioModel += Player_updateAudioModel;

            _recomendatePage     = new RecomendatePage();
            _popularPage         = new PopularPage();
            _MyAudiosPage        = new MyAudios();
            _AlbumsPage          = new AlbumsPage();
            _searchPage          = new SearchPage();
            _repostPage          = new RepostPage();
            _currentPlaylistPage = new CurrentPlaylistPage();
            AuthFrame.Visibility = Visibility.Collapsed;

            СurrentPlaylistTab.Content = _currentPlaylistPage.Content;
            MyAudiosTab.Content = _MyAudiosPage.Content;
            MyPlayliststab.Content = _AlbumsPage.Content;
            SearchTab.Content = _searchPage.Content;
            RecomendationTab.Content = _recomendatePage.Content;
            PopularTab.Content = _popularPage.Content;

            TeamplatesDictonary.AudioAddToPlaylist += TeamplatesDictonary_AudioAddToPlaylist;
            TeamplatesDictonary.AudioAdd += TeamplatesDictonary_AudioAdd;
            TeamplatesDictonary.ArtistClicked += TeamplatesDictonary_ArtistClicked;
            TeamplatesDictonary.AudioDelete += TeamplatesDictonary_AudioDeleteClicked;
            TeamplatesDictonary.AudioDownload += TeamplatesDictonary_AudioDownload;

            _repostPage.ClosedEvent += _repostPage_ClosedEvent;
            Console.WriteLine("Loaded");
            Tools.CurrentSettings.BackGroundInvoke();
        }

        private void TeamplatesDictonary_AudioDownload(AudioModel audioModel,object sender)
        {
            var AudioAwaiter = Tools.VkApi.Audio.GetByIdAsync(new string[] { audioModel.Owner_ID + "_" + audioModel.ID.ToString() + "_" + audioModel.AccessKey }).GetAwaiter();
            AudioAwaiter.OnCompleted(() =>
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFileCompleted += delegate (object sender1, System.ComponentModel.AsyncCompletedEventArgs e)
                    {

                        ((Button)sender).Content = System.Net.WebUtility.HtmlDecode("&#xE73E;");
                    };

                    try
                    {
                        var audiodataen = AudioAwaiter.GetResult().GetEnumerator();
                        audiodataen.MoveNext();
                        var audiodata = audiodataen.Current;
                        
                        webClient.DownloadFileAsync(new Uri(audiodata.Url.AbsoluteUri), Tools.CurrentSettings. VKAudioDownloadPath+"\\"+audiodata.Artist+"-"+audiodata.Title+".mp3"); ;
                    }

                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                   
                }

            });
        }

        

        private void _repostPage_ClosedEvent()
        {
            ThicknessAnimation RepostPageHideAnimation = new ThicknessAnimation();
            RepostPageHideAnimation.Completed += HidePlaylistAnimation_Completed;
            RepostPageHideAnimation.From = new Thickness(0, 0, 0, 0);
            RepostPageHideAnimation.To = new Thickness(0, 0, 0, this.ActualHeight);
            RepostPageHideAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4));

            AuthFrame.BeginAnimation(MarginProperty, RepostPageHideAnimation);
        }

        private void HidePlaylistAnimation_Completed(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
             {
                 AuthFrame.Visibility = Visibility.Hidden;
             }));
        }

        private void Player_updateAudioModel(AudioModel audioModel)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                PlayPauseTrackButton.Content = System.Net.WebUtility.HtmlDecode("&#xE769;");
                AudioPlayerTitle.Text = audioModel.Title;
                AudioPlayerArtist.Text = audioModel.Artist;
                if (audioModel.ImageUrl != null)
                    AudioPlayerImage.ImageSource = new BitmapImage(new Uri(audioModel.ImageUrl));
                else AudioPlayerImage.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resources/MusicIcon.jpg", UriKind.Absolute));

                PlayerSlider.Maximum = audioModel.DurationSeconds;
            }));
        }

        private void Player_UpdatePosition(int value)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                PlayerSlider.Value = value;
                CurrentDurationTextBlock.Text = durationconverter.Convert(value, null, null, null).ToString();
                PlayerSlider.SelectionEnd = value;
            }));
        }

        private void PreviousTrackButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPlaylistPage.PreviousAudio();
        }

        private void NextTrackButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPlaylistPage.NextAudio();
        }

        private void SetRepeatState()
        {
            if (!Player.IsRepeat)
            {
                RepeatButton.Content = System.Net.WebUtility.HtmlDecode("&#xE8ED;");
                Player.IsRepeat = true;
            }
            else
            {
                RepeatButton.Content = System.Net.WebUtility.HtmlDecode("&#xE8EE;");
                Player.IsRepeat = false;
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            SetRepeatState();
        }

        private void RepostAudio_Click(object sender, RoutedEventArgs e)
        {
            AuthFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            _repostPage.LoadingFriends();
            AuthFrame.Content = _repostPage.Content;

            ThicknessAnimation RepostPageShowAnimation = new ThicknessAnimation();

            RepostPageShowAnimation.From = new Thickness(0, 0, 0, this.Height);
            RepostPageShowAnimation.To = new Thickness(0, 0, 0, 0);
            RepostPageShowAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            AuthFrame.BeginAnimation(MarginProperty, RepostPageShowAnimation);
            _repostPage.SuccesLoadPanel.Visibility = Visibility.Visible;
            AuthFrame.Visibility = Visibility.Visible;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VolumeSlider.SelectionEnd = e.NewValue;
            Player.SetVolume(e.NewValue);
        }

        private void PlayerSlider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlayerSlider.SelectionEnd = e.NewValue;
            PlayerSlider.SelectionStart = 0;
        }

        private void PlayPauseState()
        {
            if (Player.Audio != null)
            {
                if (Player.IsPaused)
                {
                    PlayPauseTrackButton.Content = System.Net.WebUtility.HtmlDecode("&#xE769;");
                    Player.Play();
                }
                else
                {
                    PlayPauseTrackButton.Content = System.Net.WebUtility.HtmlDecode("&#xE768;");
                    Player.Pause();
                }
            }
        }

        private void PlayPauseTrackButton_Click(object sender, RoutedEventArgs e)
        {
            PlayPauseState();
        }

        private void PlayerSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            Track _track = PlayerSlider.Template.FindName("PART_Track", PlayerSlider) as Track;
            Player.SetPosition(_track.ValueFromPoint(e.GetPosition(PlayerSlider)));
        }

        private void СurrentPlaylistTab_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _currentPlaylistPage.ScrollingToActiveAudio();
        }
       

        private void MinMax()
        {
            if (!Minimized)
            {
                Minimized = true;
                ResizeMode = ResizeMode.NoResize;
                this.Topmost = true;
                ShowInTaskbar = false;
                MinHeight = 100;
                Height = 100;
                Width = 600;
            }
            else
            {
                ResizeMode = ResizeMode.CanResize;
                this.Topmost = false;

                ShowInTaskbar = true;
                Height = OldHeight;
                Width = OldWith;
                Minimized = false;
            }
        }

        private void MiniMaxPlayerButton_Click(object sender, RoutedEventArgs e) =>
            MinMax();

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!Minimized)
            {
                OldWith = e.NewSize.Width;
                OldHeight = e.NewSize.Height;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsPage settingsPage = new SettingsPage();

                settingsPage.WindowStyle = WindowStyle.SingleBorderWindow;

                settingsPage.ShowDialog();

               

                Tools.SaveSettings();
            }
            catch (Exception ex)
            {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hotKeyHost = new HotKeyHost((HwndSource)PresentationSource.FromVisual(this));

            hotKeyHost.AddHotKey(new CustomHotKey(Key.P, ModifierKeys.Alt, delegate
            {
                PlayPauseState();
            }
            ));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.Right, ModifierKeys.Alt, delegate
             {
                 _currentPlaylistPage.NextAudio();
             }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.Left, ModifierKeys.Alt, delegate
            {
                _currentPlaylistPage.PreviousAudio();
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.Up, ModifierKeys.Alt, delegate
            {
                VolumeSlider.Value += 0.1;
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.Down, ModifierKeys.Alt, delegate
            {
                VolumeSlider.Value -= 0.1;
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.F, ModifierKeys.Alt, delegate
            {
                MinMax();
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.R, ModifierKeys.Alt, delegate
            {
                SetRepeatState();
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.V, ModifierKeys.Alt, delegate
            {
                if (IsHidden)
                {
                    Hide();
                    Show();
                    Topmost = true;
                    Topmost = false;
                    IsHidden = false;
                }
                else { Hide(); IsHidden = true; }
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.D, ModifierKeys.Alt, delegate
            {
                this.Left += 10;
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.A, ModifierKeys.Alt, delegate
            {
                this.Left -= 10;
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.W, ModifierKeys.Alt, delegate
            {
                this.Top -= 10;
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.S, ModifierKeys.Alt, delegate
            {
                this.Top += 10;
            }));
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPlaylistPage.Shuffle();
        }
    }
}