using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Fluent;

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
        public static class loggingHandler{
            public static Logger Log { get; set; }
            
            public static ServiceCollection Services;
            static loggingHandler()
            {

                Services = new ServiceCollection();

                // Регистрация логгера
                Services.AddSingleton<ILoggerFactory, LoggerFactory>();
                Services.AddSingleton(  typeof(ILogger<>), typeof(Logger<>));
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
    }
}
