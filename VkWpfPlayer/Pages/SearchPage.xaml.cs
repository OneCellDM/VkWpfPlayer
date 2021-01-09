using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer.Pages
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        public delegate void AudioAdded(AudioModel audioModel);
        public event AudioAdded AudioAdd;
        public int offset = 0;
        bool Loading = false;
        public bool AllAudioFound = false;
        public ObservableCollection<AudioModel> SearchCollection = new System.Collections.ObjectModel.ObservableCollection<AudioModel>();
        public SearchPage()
        {
            InitializeComponent();
            
            AudioListView.ItemsSource = SearchCollection;
            SuccesLoadPanel.Visibility = Visibility.Collapsed;

            ErrorDialog.Visibility = System.Windows.Visibility.Collapsed;
        }
        public void LoadAudios()
        {
            ErrorDialog.Visibility = Visibility.Collapsed;
           
                SuccesLoadPanel.Visibility = Visibility.Visible;

            var awaiter = Tools.VkApi.Audio.SearchAsync(new VkNet.Model.RequestParams.AudioSearchParams()
            {
                Count = 200,
                Offset = offset,
                Query = SearchTextBox.Text,
            }).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                try
                {
                    if (awaiter.GetResult().Count > 0)
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                        {
                            Tools.AddDataToObservationCollection(SearchCollection, awaiter.GetResult());
                             SuccesLoadPanel.Visibility = Visibility.Collapsed;
                            
                            offset += 200;
                        }));
                    else
                    {
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                        {
                            SuccesLoadPanel.Visibility = Visibility.Collapsed;
                        }));
                        AllAudioFound = true;
                    }
                }
                catch (Exception ex)
                {
                    Tools.loggingHandler.Log.Error(ex);
                    ErrorDialog.Visibility = System.Windows.Visibility.Visible;
                }
                Loading = false;
            });

        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            offset = 0;
            SearchCollection.Clear();
            if (SearchTextBox.Text.Length > 0)
                LoadAudios();
        }
        private void AudioListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void AudioListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            if (!Loading)
              if (e.VerticalChange > 0)
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                  if (!AllAudioFound)
                {
                  LoadAudios();
                Loading = true;
            }
        }

        private void AudioListView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AudioListView.SelectedAudioPlay();
        }

      

        private void ErrorDialog_Accepted()
        {
            ErrorDialog.Visibility = Visibility.Collapsed;
            offset = 0;
            SearchCollection.Clear();
            if (SearchTextBox.Text.Length > 0)
                LoadAudios();
        }
    }

}
