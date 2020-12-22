﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool Minimized = false;
        private double OldWith = 0;
        private double OldHeight = 0;

        private MyAudios _MyAudiosPage;
        private AlbumsPage _AlbumsPage;
        private SearchPage _searchPage;
        private CurrentPlaylistPage _currentPlaylistPage;
        private RepostPage _repostPage;
        private DurationConverter durationconverter = new DurationConverter();

        public MainWindow()
        {
            InitializeComponent();
            String AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VKM";
            if (!new System.IO.DirectoryInfo(AppData).Exists)
                new System.IO.DirectoryInfo(AppData).Create();

            AuthFrame.Content = new VkLogin().Content;
           
            ToolsAndsettings.AuthorizedAcces += ToolsAndsettings_AuthorizedAcces;
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
            СurrentPlaylistTab.Content = _currentPlaylistPage.Content;
            MyAudiosTab.Content = _MyAudiosPage.Content;
            MyPlayliststab.Content = _AlbumsPage.Content;
            SearchTab.Content = _searchPage.Content;
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

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Player.IsRepeat)
            {
                RepeatButton.Foreground = new BrushConverter().ConvertFromString("#FF4A76A8") as Brush;
                Player.IsRepeat = true;
            }
            else
            {
                RepeatButton.Foreground = new BrushConverter().ConvertFromString("#FF000000") as Brush;
                Player.IsRepeat = false;
            }
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

        private void PlayPauseTrackButton_Click(object sender, RoutedEventArgs e)
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

        private void PlayerSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            Track _track = PlayerSlider.Template.FindName("PART_Track", PlayerSlider) as Track;
            Player.SetPosition(_track.ValueFromPoint(e.GetPosition(PlayerSlider)));
        }

        private void СurrentPlaylistTab_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _currentPlaylistPage.ScrollingToActiveAudio();
        }

        private void MiniMaxPlayerButton_Click(object sender, RoutedEventArgs e)
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
                ToolsAndsettings.CurrentSettings.TextColor = settingsPage.TextColoTextbox.Text.ToUpper().Trim();
                ToolsAndsettings.CurrentSettings.ConrolColor = settingsPage.ControlColorTextbox.Text.ToUpper().Trim();
                ToolsAndsettings.CurrentSettings.ButtonColor = settingsPage.ButtonColorTextbox.Text.ToUpper().Trim();
                ToolsAndsettings.CurrentSettings.ImageBorderColor = settingsPage.ImageBorderColorTextBox.Text.ToUpper().Trim();
                ToolsAndsettings.CurrentSettings.BackGroundColor = settingsPage.BackGroundTextBox.Text.ToUpper().Trim();
                ToolsAndsettings.CurrentSettings.SliderColor = settingsPage.SliderColorsTextBox.Text.ToUpper().Trim();
            }
            catch(Exception ex)
            {
               
            }
        }
    }
}