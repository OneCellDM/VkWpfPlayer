using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer
{
    /// <summary>
    /// Логика взаимодействия для MyAudios.xaml
    /// </summary>
    public partial class MyAudios : Page
    {

        public delegate void AudioSLoaded();

        public event AudioSLoaded SUCESS;
        Task LoadTask;



        public ObservableCollection<AudioModel> SearchCollection = new System.Collections.ObjectModel.ObservableCollection<AudioModel>();



        public ObservableCollection<AudioModel> AudioCollection = new System.Collections.ObjectModel.ObservableCollection<AudioModel>();

        public MyAudios()
        {
            InitializeComponent();
            AudioListView.Items.Clear();
            AudioListView.ItemsSource = AudioCollection;
            StartLoading();
            ErrorGrid.Visibility = System.Windows.Visibility.Collapsed;

        }
        private void StartLoading()
        {
            if (LoadTask != null && LoadTask.Status == TaskStatus.Running)
            {

                LoadTask.Dispose();
            }
            LoadTask = Task.Run(() =>
            {
                LoadAudios();

            });

        }

        public void LoadAudios()
        {

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                SuccesLoadPanel.Visibility = System.Windows.Visibility.Visible;
            }));

            var awaiter = ToolsAndsettings.VkApi.Audio.GetAsync(new VkNet.Model.RequestParams.AudioGetParams()
            {
                Count = 6000,

            });
            awaiter.GetAwaiter().OnCompleted(() =>
            {

                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    try
                    {

                        ToolsAndsettings.AddDataToObservationCollection(AudioCollection, awaiter.GetAwaiter().GetResult());
                        SuccesLoadPanel.Visibility = System.Windows.Visibility.Collapsed;


                    }
                    catch (Exception ex)
                    {

                        ToolsAndsettings.loggingHandler.Log.Error(ex);
                        ErrorGrid.Visibility = System.Windows.Visibility.Visible;
                    }

                }));

            });

        }

        private void AudioListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                ToolsAndsettings.SendListClickEvent((ObservableCollection<AudioModel>)AudioListView.ItemsSource, AudioListView.SelectedIndex);
                Player.Play(((AudioModel)e.AddedItems[e.AddedItems.Count - 1]));
            }
        }
        private void search()
        {
            SearchCollection.Clear();
            var list = AudioCollection.Where(x => ((AudioModel)x).Title.ToLower().Contains(SearchTextBox.Text.ToLower()) |
                ((AudioModel)x).Artist.ToLower().Contains(SearchTextBox.Text.ToLower())
                 ).GroupBy(
                         x => ((AudioModel)x).Title).
                         Select(x => x.First()
                         ).ToList();
            foreach (var audio in list)
                SearchCollection.Add(audio);

            AudioListView.ItemsSource = SearchCollection;
        }
        private void SearchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            search();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text.Length == 0)
                AudioListView.ItemsSource = AudioCollection;
            else
            {
                search();

            }
        }

        private void RetryRequests_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StartLoading();
            ErrorGrid.Visibility = System.Windows.Visibility.Collapsed;

        }
    }
}