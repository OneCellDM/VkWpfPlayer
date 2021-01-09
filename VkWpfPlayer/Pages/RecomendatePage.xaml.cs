using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer.Pages
{
    /// <summary>
    /// Логика взаимодействия для RecomendatePage.xaml
    /// </summary>
    public partial class RecomendatePage : Page
    {

        ObservableCollection<AudioModel> AudioCollection = new System.Collections.ObjectModel.ObservableCollection<AudioModel>();
        Thread Tloading;
        bool erro = false;
        bool isloading = false;
        public RecomendatePage()
        {
            InitializeComponent();
            AudioListview.ItemsSource = AudioCollection;
            ErrorDialog.Visibility = Visibility.Collapsed;
            StartLoading();



        }
        public void StartLoading()
        {

 
            if(!isloading)
            { 
                    erro = false;

                    ErrorDialog.Visibility = Visibility.Collapsed;
                    LoadingComponent.Visibility = Visibility.Visible;



                    Console.WriteLine("Start loading");
                    AudioCollection.Clear();

                    Tloading = new Thread(() => Loading());
                    Tloading.Start();
           }
               


        }
        public void Loading()
        {
            isloading = true;
            var Awaiter = Tools.VkApi.Audio.GetRecommendationsAsync(null, null, 1000, 0, false).GetAwaiter();
            Awaiter.OnCompleted(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        Tools.AddDataToObservationCollection(AudioCollection, Awaiter.GetResult());
                        LoadingComponent.Visibility = Visibility.Collapsed;

                    }
                    catch (Exception ex)
                    {
                        erro = true;
                        ErrorDialog.Visibility = Visibility.Visible;
                    }
                    isloading = false;
                });
            }); 

        }

        private void AudioListview_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(AudioListview.SelectedItems.Count>0)
            {
                Tools.SendListClickEvent(AudioCollection,AudioListview.SelectedIndex);
                Player.Play((AudioModel)AudioListview.SelectedItem);
            }
        }

        private void ErrorDialog_Accepted()
        {
            StartLoading();
        }

        private void UpdateData_Click(object sender, RoutedEventArgs e)
        {
            StartLoading();
        }
    }
}
