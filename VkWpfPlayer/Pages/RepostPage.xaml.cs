using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VkNet.Model.RequestParams;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer.Pages
{
    /// <summary>
    /// Логика взаимодействия для RepostPage.xaml
    /// </summary>
    public partial class RepostPage : Page
    {
        public delegate void Closed();

        public event Closed ClosedEvent;

        private ObservableCollection<FriendModel> FriendsCollection = new ObservableCollection<FriendModel>();
        private ObservableCollection<FriendModel> SearchCollection = new ObservableCollection<FriendModel>();

        public RepostPage()
        {
            InitializeComponent();
            SuccesLoadPanel.Visibility = Visibility.Visible;
            FriendsListview.ItemsSource = FriendsCollection;
        }
        public void LoadingFriends()
        {
            FriendsCollection.Clear();
            Task.Run(() =>
            {
                var awaiterData = ToolsAndsettings.VkApi.Friends.GetAsync(new VkNet.Model.RequestParams.FriendsGetParams()
                {
                    Fields = VkNet.Enums.Filters.ProfileFields.Photo50,
                }).GetAwaiter();
                awaiterData.OnCompleted(() =>
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                    {
                        try
                        {
                            ToolsAndsettings.AddDataToObservationCollection(FriendsCollection, awaiterData.GetResult());
                            SuccesLoadPanel.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception ex)
                        {
                            LoadingFriends();
                        }
                    }));
                });
            });
        }

        private void FriendsListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FriendsListview.SelectedIndex != -1)
            {
                var awaiterdata = ToolsAndsettings.VkApi.Messages.SendAsync(new MessagesSendParams()
                {
                    RandomId = new Random().Next(1, 99999999),
                    Attachments = ToolsAndsettings.VkApi.Audio.GetById(new String[] { Player.Audio.Owner_ID.ToString() + "_" + Player.Audio.ID + "_" + Player.Audio.AccessKey }),
                    UserId = ((FriendModel)e.AddedItems[e.AddedItems.Count - 1]).ID,

                }).GetAwaiter();
                awaiterdata.OnCompleted(() =>
                {
                    if (ClosedEvent != null)
                        ClosedEvent.Invoke();
                });
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchCollection.Clear();
            var list = FriendsCollection.Where(x => ((FriendModel)x).UserName.ToLower().Contains(SearchTextBox.Text.ToLower())
                 );
            foreach (var audio in list)
                SearchCollection.Add(audio);

            FriendsListview.ItemsSource = SearchCollection;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text.Length == 0)
                FriendsListview.ItemsSource = FriendsCollection;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClosedEvent != null)
                ClosedEvent.Invoke();
        }
    }
}