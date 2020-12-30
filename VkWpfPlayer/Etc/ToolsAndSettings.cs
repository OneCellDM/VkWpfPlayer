using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;

using System.Collections.ObjectModel;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.NLog.Extensions.Logging.Extensions;
using VkNet.Utils;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer
{
    public static class ToolsAndsettings
    {
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

        public static class CurrentSettings
        {
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

        public static void AddDataToObservationCollection(ObservableCollection<AudioModel> observableCollection, VkCollection<Audio> VkCollection)
        {
            foreach (var audio in VkCollection)
            {
                
                var model = new AudioModel()
                {
                    Artist = audio.Artist,
                    ID = (long)audio.Id,
                    DurationSeconds = audio.Duration,
                    AccessKey = audio.AccessKey,
                    Owner_ID = (long)audio.OwnerId,
                    Title = audio.Title
                };

                if (audio.Album != null && audio.Album.Thumb != null && audio.Album.Thumb.Photo68 != null)
                    model.ImageUrl = audio.Album.Thumb.Photo68;

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
        public static void AddDataToObservationCollection(ObservableCollection<AudioModel> observableCollection, AudioModel audioModel)
        {
                observableCollection.Add(audioModel);
            
        }
    }
}