using System.Collections.ObjectModel;
using System.Windows.Controls;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer
{
    /// <summary>
    /// Логика взаимодействия для CurrentPlaylistPage.xaml
    /// </summary>
    public partial class CurrentPlaylistPage : Page
    {
        private bool LoadingList = false;
        private int SelectedIndex = -1;

        public ObservableCollection<AudioModel> AudioCollection = new System.Collections.ObjectModel.ObservableCollection<AudioModel>();

        public CurrentPlaylistPage()
        {
            InitializeComponent();
            AudioListView.ItemsSource = AudioCollection;
            ToolsAndsettings.ListClickHandlerCommonEvent += ToolsAndsettings_ListClickHandlerCommonEvent;
            Player.Stopped += Player_Stopped;
        }

        private void Player_Stopped() =>
            this.Dispatcher.Invoke(() =>
        {
            NextAudio();
        });

        public void PreviewAudio()
        {
            if (AudioListView.SelectedIndex>0)
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
            if (!LoadingList)
                Player.Play(((AudioModel)e.AddedItems[e.AddedItems.Count - 1]));

            SelectedIndex = AudioListView.SelectedIndex;
        }
    }
}