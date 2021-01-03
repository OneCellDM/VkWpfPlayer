using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer.Pages
{
    /// <summary>
    /// Логика взаимодействия для PopularPage.xaml
    /// </summary>
    public partial class PopularPage : Page
    {

        private ThicknessAnimation ShowPlaylistAnimation = new ThicknessAnimation();
        private ThicknessAnimation HidePlaylistAnimation = new ThicknessAnimation();

        ObservableCollection<AudioModel> AudioCollection = new System.Collections.ObjectModel.ObservableCollection<AudioModel>();
        ObservableCollection<AlbumModel> GenreCollection = new System.Collections.ObjectModel.ObservableCollection<AlbumModel>
        {
            new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Pop,Title="Популярная музыка" },
             new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Other,Title="Другая музыка" },
              new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.AcousticAndVocal,Title="Акустическая музыка и вокал" },
               new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Alternative,Title="Альтернативная" },
                new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Chanson,Title="Шансон" },
                 new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Classical,Title="Классика" },
                  new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.DanceAndHouse,Title="Танцевальная и хаус музыка" },
                   new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.DrumAndBass,Title="Драм-Н-Басс" },
                    new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Dubstep,Title="Дабстеп" },
                     new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.EasyListening,Title="Легкая музыка" },
                      new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.ElectropopAndDisco,Title="Электро-поп и диско музыка" },
                       new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Ethnic,Title="Этническая музыка" },
                        new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.IndiePop,Title="Инди-Поп" },
                         new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Instrumental,Title="Инструментальная" },
                          new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.JazzAndBlues,Title="Джаз и Блюз" },
                           new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Metal,Title="Метал" },
                            new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.RapAndHipHop,Title="Рэп и хипхоп" },
                             new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Reggae,Title="Регги" },
                              new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Rock,Title="Рок" },
                               new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Speech,Title="Спич" },
                                new AlbumModel{ ID=(long)VkNet.Enums.AudioGenre.Trance,Title="Транс" },
        };
        public PopularPage()
        {
           
            InitializeComponent();
            AlbumsListView.ItemsSource = GenreCollection;
            AlbumGrid.Visibility = Visibility.Collapsed;
            SuccesLoadPanel.Visibility = Visibility.Collapsed;
            AudioListView.ItemsSource = AudioCollection;
        }

        private void AlbumsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlbumsListView.SelectedIndex != -1)
            {
                
                var Await = ToolsAndsettings.VkApi.Audio.GetPopularAsync(false, (VkNet.Enums.AudioGenre)((AlbumModel)AlbumsListView.SelectedItem).ID, 1000, 0).GetAwaiter();
                Await.OnCompleted(() =>
                {

                    ToolsAndsettings.AddDataToObservationCollection(AudioCollection, Await.GetResult());
                    SuccesLoadPanel.Visibility = Visibility.Collapsed;



                });
                AlbumTitleText.Text = ((AlbumModel)AlbumsListView.SelectedItem).Title;
               

                AlbumsListView.SelectedIndex = -1;
                SuccesLoadPanel.Visibility = Visibility.Visible;
                AlbumGrid.Visibility = Visibility.Visible;


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

        private void AudioListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(AudioListView.SelectedItems.Count>0)
            {
                Player.Play( (AudioModel) AudioListView.SelectedItem);
                ToolsAndsettings.SendListClickEvent(AudioCollection, AudioListView.SelectedIndex);
            }
        }
    }
}
