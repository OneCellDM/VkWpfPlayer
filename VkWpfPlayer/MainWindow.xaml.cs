using System;
using System.Diagnostics;
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
        private  Converters.DurationConverter durationconverter = new Converters.DurationConverter();

        public MainWindow()
        {
            InitializeComponent();
            String AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VKM";
            if (!new System.IO.DirectoryInfo(AppData).Exists)
                new System.IO.DirectoryInfo(AppData).Create();

            AuthFrame.Content = new VkLogin().Content;
            TeamplatesDictonary.ArtistClicked += TeamplatesDictonary_ArtistClicked;
            TeamplatesDictonary.AudioDeleteClicked += TeamplatesDictonary_AudioDeleteClicked;

            ToolsAndsettings.AuthorizedAcces += ToolsAndsettings_AuthorizedAcces;

           

        }

        private void TeamplatesDictonary_AudioDeleteClicked(object audioModel)
        {
           
            var taskAwaiter = ToolsAndsettings.VkApi.Audio.DeleteAsync(((AudioModel)audioModel).ID, ((AudioModel)audioModel).Owner_ID).GetAwaiter();
            taskAwaiter.OnCompleted(() =>
            {

                var taskAwaiter2 = ToolsAndsettings.VkApi.Audio.GetByIdAsync(new String[]
                { ((AudioModel)audioModel).ID.ToString() + "_" + ((AudioModel)audioModel).Owner_ID.ToString()}).GetAwaiter();
                taskAwaiter2.OnCompleted(() =>
                {
                    try
                    {
                        var res = taskAwaiter2.GetResult();
                    }
                    catch(VkNet.Exception.ParameterMissingOrInvalidException ex)
                    {
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                        {
                            if (MyAudiosTab.IsSelected)
                                _MyAudiosPage.AudioCollection.Remove((AudioModel)audioModel);

                            if (MyPlayliststab.IsSelected)
                                _AlbumsPage.AudioCollection.Remove((AudioModel)audioModel);

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

            _MyAudiosPage = new MyAudios();
            _AlbumsPage = new AlbumsPage();
            _searchPage = new SearchPage();
            _repostPage = new RepostPage();
            _currentPlaylistPage = new CurrentPlaylistPage();
            AuthFrame.Visibility = Visibility.Collapsed;

            _repostPage.ClosedEvent += _repostPage_ClosedEvent;
            _searchPage.AudioAdd += _searchPage_AudioAdd;
            СurrentPlaylistTab.Content = _currentPlaylistPage.Content;
            MyAudiosTab.Content = _MyAudiosPage.Content;
            MyPlayliststab.Content = _AlbumsPage.Content;
            SearchTab.Content = _searchPage.Content;
        }

        private void _searchPage_AudioAdd(AudioModel audioModel)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
             {
                 ToolsAndsettings.AddDataToObservationCollection(_MyAudiosPage.AudioCollection, audioModel);
                 _MyAudiosPage.AudioCollection.Move(_MyAudiosPage.AudioCollection.Count - 1, 0);
             }));
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

        private void PreviewTrackButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPlaylistPage.PreviewAudio();
        }

        private void PauseTrackButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void NextTrackButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPlaylistPage.NextAudio();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
        private void MiniMaxPlayerButton_Click(object sender, RoutedEventArgs e)=>
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
            settingsPage.WindowStyle = WindowStyle.ToolWindow;

            settingsPage.ShowDialog();

            ToolsAndsettings.CurrentSettings.ImageCornerRadios = settingsPage.RoundImageSlider.Value;
            ToolsAndsettings.CurrentSettings.ButtonAndTextBoxCornerRadius = settingsPage.ButtonRoundRadiusSlider.Value;
            ToolsAndsettings.CurrentSettings.ImageBorderThickness = settingsPage.BorderThicknessSlider.Value;
            ToolsAndsettings.CurrentSettings.TextColor = settingsPage.GetTextFromTextbox(settingsPage.TextColoTextbox);
            ToolsAndsettings.CurrentSettings.ControlColor = settingsPage.GetTextFromTextbox(settingsPage.ControlColorTextbox);
            ToolsAndsettings.CurrentSettings.ButtonColor = settingsPage.GetTextFromTextbox(settingsPage.TextBoxAndButtonColorTextbox);
            ToolsAndsettings.CurrentSettings.ImageBorderColor = settingsPage.GetTextFromTextbox(settingsPage.ImageBorderColorTextBox);
            ToolsAndsettings.CurrentSettings.BackGroundColor = settingsPage.GetTextFromTextbox(settingsPage.BackGroundTextBox);
            ToolsAndsettings.CurrentSettings.SliderColor = settingsPage.GetTextFromTextbox(settingsPage.SliderColorsTextBox);
            ToolsAndsettings.CurrentSettings.TextBoxColor = settingsPage.GetTextFromTextbox(settingsPage.TextBoxAndButtonColorTextbox);
            ToolsAndsettings.CurrentSettings.PlayerButtonTextColor = settingsPage.GetTextFromTextbox(settingsPage.PlayerButtonTextColorTextBox);
        }
        catch (Exception ex)
        {

        }
    }
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hotKeyHost = new HotKeyHost((HwndSource)PresentationSource.FromVisual(this));

            hotKeyHost.AddHotKey(new CustomHotKey(Key.P, ModifierKeys.Alt,delegate {
                PlayPauseState();
            }
            ));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.Right, ModifierKeys.Alt,delegate
            {
                _currentPlaylistPage.NextAudio();
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.Left, ModifierKeys.Alt, delegate
            {
                _currentPlaylistPage.PreviewAudio();
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.Up, ModifierKeys.Alt, delegate
            {
                VolumeSlider.Value+=0.1;
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.Down, ModifierKeys.Alt, delegate
            {
                VolumeSlider.Value-=0.1;
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
                this.Left+=10;
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.A, ModifierKeys.Alt, delegate
            {
                this.Left-=10;
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.W, ModifierKeys.Alt, delegate
            {
                this.Top-=10;
            }));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.S, ModifierKeys.Alt, delegate
            {
                this.Top+=10;
            }));


        }



    }
}