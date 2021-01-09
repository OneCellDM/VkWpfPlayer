using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer.Pages
{
    /// <summary>
    /// Логика взаимодействия для CurrentPlaylistPage.xaml
    /// </summary>
    public partial class CurrentPlaylistPage : Page
    {
        private bool LoadingList = false;
        private int SelectedIndex = -1;
        public bool Shuffling = false;

        public ObservableCollection<AudioModel> AudioCollection = new System.Collections.ObjectModel.ObservableCollection<AudioModel>();
      
        public void Shuffle()
        {
             Shuffling = true;
             Random rand = new Random(DateTime.Now.Millisecond);
            
                for (int i = 0; i < AudioCollection.Count - 1; i++)
                {
                    int randa = rand.Next(AudioCollection.Count);
                    int randb = rand.Next(AudioCollection.Count);
                    var buf = AudioCollection[randa];
                    AudioCollection[randa] = AudioCollection[randb];
                    AudioCollection[randb] = buf;
                }
             Shuffling = false;   
        }

        public CurrentPlaylistPage()
        {
            InitializeComponent();
            AudioListView.ItemsSource = AudioCollection;
            Tools.ListClickHandlerCommonEvent += ToolsAndsettings_ListClickHandlerCommonEvent;
            Player.Stopped += Player_Stopped;
        }

        private void Player_Stopped() =>
            this.Dispatcher.Invoke(() =>
        {
            NextAudio();
        });

        public void PreviousAudio()
        {
            if (AudioListView.SelectedIndex > 0)
                AudioListView.SelectedIndex = AudioListView.SelectedIndex - 1;
        }

        public void NextAudio()
        {
            if (AudioListView.SelectedIndex != AudioCollection.Count - 1)
                AudioListView.SelectedIndex = AudioListView.SelectedIndex + 1;
        }

        public void ScrollingToActiveAudio()
        {
            if (AudioCollection.Count > 0)
                AudioListView.ScrollIntoView(AudioCollection[SelectedIndex]);
        }

        private void ToolsAndsettings_ListClickHandlerCommonEvent(System.Collections.ObjectModel.ObservableCollection<AudioModel> model, int selectedIndex)
        {
            LoadingList = true;
            if (selectedIndex != -1)
            {
                SelectedIndex = selectedIndex;

                AudioModel[] AudiomodelArry = new AudioModel[model.Count];
                model.CopyTo(AudiomodelArry, 0);
                AudioCollection.Clear();
                foreach (var Audio in AudiomodelArry)
                    AudioCollection.Add(Audio);

                AudioListView.SelectedIndex = selectedIndex;
                AudioListView.ScrollIntoView(AudioCollection[SelectedIndex]);
            }
            LoadingList = false;
        }

        private void AudioListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Shuffling)
            {
                if (!LoadingList)
                    Player.Play(((AudioModel)e.AddedItems[e.AddedItems.Count - 1]));

                SelectedIndex = AudioListView.SelectedIndex;
            }
        }
    }
}