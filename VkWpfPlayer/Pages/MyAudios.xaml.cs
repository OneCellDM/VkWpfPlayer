using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer.Pages
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

            

        }
        private void StartLoading()
        {
            ErrorDialog.Visibility = System.Windows.Visibility.Collapsed;
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
                SearchTextBox.Text = "";
                AudioCollection.Clear();
                Tools.UI.ShowElements(SuccesLoadPanel);
            }));
         

                var awaiter = Tools.VkApi.Audio.GetAsync(new VkNet.Model.RequestParams.AudioGetParams()
                {
                    Count = 6000,

                });
                awaiter.GetAwaiter().OnCompleted(() =>
                {

                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                    {
                        try
                        {
                            SuccesLoadPanel.Visibility = System.Windows.Visibility.Collapsed;
                            Tools.AddDataToObservationCollection(AudioCollection, awaiter.GetAwaiter().GetResult());
                            
                        }
                        catch (Exception ex)
                        {

                            Tools.loggingHandler.Log.Error(ex);
                            Tools.UI.ShowElements(ErrorDialog);

                        }
                    }));

                });
            
          

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

     
        private void AudioListView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AudioListView.SelectedAudioPlay();
        }

    
           
        

        private void ErrorDialog_Accepted()
        {
            StartLoading();
        }

        private void ErrorDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void UpdateListButton_Click(object sender, System.Windows.RoutedEventArgs e)
        { 
            StartLoading();
        }
    }
}