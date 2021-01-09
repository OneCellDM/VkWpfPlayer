using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.NLog.Extensions.Logging.Extensions;
using VkNet.Utils;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer
{

    public static class Tools
    {


        static Tools()
        {

            CurrentSettings.VKAudioDownloadPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\VKM\\AUDIO";
            if (!new DirectoryInfo(CurrentSettings.VKAudioDownloadPath).Exists)
                new DirectoryInfo(CurrentSettings.VKAudioDownloadPath).Create();

            CurrentSettings.CachePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\VKM\\CACHE";

            if (!new DirectoryInfo(CurrentSettings.CachePath).Exists)
                new DirectoryInfo(CurrentSettings.CachePath).Create();

            LoadSettings();
        }
        public static void SelectedAudioPlay(this System.Windows.Controls.ListView listView)
        {
            if (listView.SelectedItems.Count > 0)
            {
                SendListClickEvent((ObservableCollection<AudioModel>)listView.ItemsSource, listView.SelectedIndex);
                Player.Play(((AudioModel)(listView).SelectedItem));
            }
        }
        public static class UI
        {
         
            public static void HideElements(params FrameworkElement[] hideElement)
            {

                foreach (var element in hideElement)
                    element.Visibility = Visibility.Collapsed;
                
            }
           
            public static void ShowElements(params FrameworkElement[] showElement)
            {
                foreach(var element in showElement)
                    element.Visibility = Visibility.Visible;
                
                
            }


        }

        public static class DefaultSettings
        {
            
            public static double ImageCornerRadios { get; private set; } = 10;
            public static double ButtonAndTextBoxCornerRadius { get; private set; } = 6;

            public static string BackGroundColor { get; private set; } = "white";
            public static string TextColor { get; private set; } = "black";

            public static string MouseOverColor { get; private set; } = "#4A76A8";
            public static string ButtonColor { get; private set; } = "#EDEEF0";
            public static string TextBoxColor { get; private set; } = "#EDEEF0";
            public static string SliderColor { get; private set; } = "#EDEEF0";
            public static string ControlColor { get; private set; } = "#4A76A8";
            public static string ImageBorderColor { get; private set; } = "transparent";
            public static string PlayerButtonTextColor { get; set; } = "white";
            public static int ImageBorderThickness { get; private set; } = 0;
            public static int ButtonAndTextBoxBorderThickness { get; private set; } = 2;
        }
        public static void SaveSettings()
        {
            var fields = typeof(CurrentSettings).GetProperties(BindingFlags.Static | BindingFlags.Public);
                object[,] a = new object[fields.Length, 2];
                int i = 0;
                foreach (PropertyInfo field in fields)
                {
                    a[i, 0] = field.Name;
                    a[i, 1] = field.GetValue(null);
                    i++;
                };
                Stream f = File.Open("settings.json", FileMode.Create);
            var s = new StreamWriter(f);
            s.Write(JsonConvert.SerializeObject(a));
            s.Close();    
        }

        public static void LoadSettings()
        {
           
            if (File.Exists("settings.json"))
            {
                var fields = typeof(CurrentSettings).GetProperties(BindingFlags.Static | BindingFlags.Public);
                object[,] a;
                Stream f = File.Open("settings.json", FileMode.Open);

                var rd = new StreamReader(f);

                a = JsonConvert.DeserializeObject<object[,]>(rd.ReadToEnd()) as object[,];

                rd.Close();
                f.Close();

                int i = 0;
                foreach (var field in fields)
                {
                       
                    if (field.Name == (a[i, 0] as string))
                    {
                        field.SetValue(null, a[i, 1]);
                    }
                    i++;
                };
                CurrentSettings.SetCornerRadius("VKImageCornerRadius", (int)CurrentSettings.ImageCornerRadios);
                CurrentSettings.SetColor("VKTextColor", Tools.CurrentSettings.TextColor);
                CurrentSettings.SetColor("VKBackGroundColor", CurrentSettings.BackGroundColor);
                CurrentSettings.SetCornerRadius("VKButtonAndTextBoxCornerRadius",(int) CurrentSettings.ButtonAndTextBoxCornerRadius);
                CurrentSettings.SetColor("VkMouseOverColor", CurrentSettings.MouseOverColor);
                CurrentSettings.SetColor("VkButtonColor", CurrentSettings.TextBoxColor);
                CurrentSettings.SetColor("VkTextBoxColor", CurrentSettings.TextBoxColor);
                CurrentSettings.SetColor("VkContolColor", CurrentSettings.ControlColor);
                CurrentSettings.SetColor("VkSliderColor", CurrentSettings.SliderColor);
                CurrentSettings.SetBorderThickness("VKImageBorderThicknes",(int)CurrentSettings.ImageBorderThickness);
                CurrentSettings.SetColor("VKPlayerButtonTextColor", CurrentSettings.PlayerButtonTextColor);
                   
             
            }
        }
        

        public static class CurrentSettings
        {
            public delegate void UpdateBackground();
            public static event UpdateBackground BackGroundChanged;




            public static double BackGroundOpacity { get; set; } = 1;
            public static bool IsBackgroundImage { get; set; }
            public static string BackGroundImage { get; set; }
            public static string VKAudioDownloadPath { get; set; }
            public static string CachePath { get; set; }

            private  static BrushConverter brushConverter = new BrushConverter();
            public static void SetColor(String resourceName, String colorName) =>
                    System.Windows.Application.Current.Resources[resourceName] = brushConverter.ConvertFromString(colorName);
            public static void SetCornerRadius(String resourceName, int value) =>
                System.Windows.Application.Current.Resources[resourceName] = new System.Windows.CornerRadius(value);

            public static String GetTextFromTextbox(object textBox) =>
                ((System.Windows.Controls.TextBox)textBox).Text.ToUpper().Trim();

            public static void SetBorderThickness(String resourceName, int value) =>
                 System.Windows.Application.Current.Resources[resourceName] = new System.Windows.Thickness(value);
           public static void BackGroundInvoke()
            {
                BackGroundChanged?.Invoke();
            }
            private static bool ValidateColor(String Name)
            {
                try
                {
                    
                    brushConverter.ConvertFromString(Name);
                    return true;

                }
                catch (Exception ex) { return false; }

            }

            public static double ImageCornerRadios { get; set; } = DefaultSettings.ImageCornerRadios;
            public static double ButtonAndTextBoxCornerRadius { get; set; } = DefaultSettings.ButtonAndTextBoxCornerRadius;

            public static string BackGroundColor { get; set; } = DefaultSettings.BackGroundColor;
            public static string TextColor      { get; set; } = DefaultSettings.TextColor;

            public static string MouseOverColor { get; set; } = DefaultSettings.MouseOverColor;
            public static string ButtonColor { get; set; } = DefaultSettings.ButtonColor;
            public static string TextBoxColor { get; set; } = DefaultSettings.TextBoxColor;
            public static string SliderColor { get; set; } = DefaultSettings.SliderColor;
            public static string ControlColor { get; set; } = DefaultSettings.ControlColor;
            public static string PlayerButtonTextColor { get; set; } = DefaultSettings.PlayerButtonTextColor;
            public static string ImageBorderColor { get; set; } = DefaultSettings.ImageBorderColor;

            public static double ImageBorderThickness { get; set; } = DefaultSettings.ImageBorderThickness;
            public static double ButtonAndTextBoxBorderThickness { get; set; } = DefaultSettings.ButtonAndTextBoxBorderThickness;

           
        }

        public static class loggingHandler
        {
            public static Logger Log { get; set; }

            public static ServiceCollection Services;

            static loggingHandler()
            {
                Services = new ServiceCollection();

                // Регистрация логгера
                Services.AddSingleton<ILoggerFactory, LoggerFactory>();
                Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                Services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    builder.AddNLog(new VkNet.NLog.Extensions.Logging.NLogProviderOptions
                    {
                        CaptureMessageProperties = true,
                        CaptureMessageTemplates = true
                    });
                });
                NLog.LogManager.LoadConfiguration("nlog.config");
                Log = NLog.LogManager.GetCurrentClassLogger();
            }
        }

        public delegate void ListClickHandlerCommon(ObservableCollection<AudioModel> model, int selectedIndex);

        public static event ListClickHandlerCommon ListClickHandlerCommonEvent;

        private static VkNet.VkApi vk;

        public delegate void Authorized();

        public static event Authorized AuthorizedAcces;

        public static VkNet.VkApi VkApi
        {
            get { return vk; }
            set
            {
                vk = value;

                if (AuthorizedAcces != null)
                    AuthorizedAcces();
            }
        }

        public static void SendListClickEvent(ObservableCollection<AudioModel> models, int SelectedIndex)
        {
            if (ListClickHandlerCommonEvent != null)
                ListClickHandlerCommonEvent.Invoke(models, SelectedIndex);
        }
        public static void AddDataToObservationCollection(ObservableCollection<FriendModel> observableCollection, VkCollection<User> VkCollection)
        {
            foreach (var user in VkCollection)
            {
                var model = new FriendModel()
                {
                    AvatarUrl = user.Photo50.AbsoluteUri,
                    UserName = user.FirstName + " " + user.LastName,
                    ID = user.Id,
                };
                observableCollection.Add(model);
            }
        }

        public static void AddDataToObservationCollection(ObservableCollection<AlbumModel> observableCollection, VkCollection<AudioPlaylist> VkCollection)
        {
            foreach (var album in VkCollection)
                if (album.Photo != null && album.Photo.Photo135 != null)

                    observableCollection.Add(new AlbumModel()
                    {
                        OwnerID = (long)album.OwnerId,
                        ID = (long)album.Id,
                        ImageUrl = album.Photo.Photo135,
                        Title = album.Title
                    });
                else if (album.Thumbs != null && album.Thumbs[0] != null && album.Thumbs[0].Photo135 != null)
                    observableCollection.Add(new AlbumModel()
                    {
                        ID = (long)album.Id,
                        ImageUrl = album.Thumbs[0].Photo135,
                        Title = album.Title,
                        OwnerID = (long)album.OwnerId,
                    });
                else
                {
                    observableCollection.Add(new AlbumModel()
                    {
                        ID = (long)album.Id,
                        OwnerID = (long)album.OwnerId,

                        Title = album.Title
                    });
                }
        }
        public static void AddDataToObservationCollection(ObservableCollection<AudioModel> observableCollection, VkCollection<Audio> VkCollection)
        {
            foreach (var audio in VkCollection)
                AddDataToObservationCollection(observableCollection, audio);
            
        }
        public static void AddDataToObservationCollection(ObservableCollection<AudioModel> observableCollection, Audio VkAudio)
        {
            var model = new AudioModel()
            {
                Artist = VkAudio.Artist,
                ID = (long)VkAudio.Id,
                DurationSeconds = VkAudio.Duration,
                AccessKey = VkAudio.AccessKey,
                Owner_ID = (long)VkAudio.OwnerId,
                Title = VkAudio.Title
            };

            if (VkAudio.Album != null && VkAudio.Album.Thumb != null && VkAudio.Album.Thumb.Photo68 != null)
                model.ImageUrl = VkAudio.Album.Thumb.Photo68;

            AddDataToObservationCollection(observableCollection, model);

        }

        public static void AddDataToObservationCollection(ObservableCollection<AudioModel> observableCollection, AudioModel audioModel)=>
                observableCollection.Add(audioModel);
            
        
        public static void AddDataToObservationCollection(ObservableCollection<AudioModel> observableCollection, IEnumerable<Audio> EnumerableCollections)
        {
            var iter = EnumerableCollections.GetEnumerator();

            while (iter.MoveNext())
            {
                var iterValue = iter.Current;
                AddDataToObservationCollection(observableCollection, iterValue);
            }
        }
    }
}