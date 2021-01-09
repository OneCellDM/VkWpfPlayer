using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace VkWpfPlayer.DataModels
{
    public class AlbumModel : INotifyPropertyChanged
    {
        public string image;
        public long ID { get; set; }
        public string Title { get; set; }
        public string ImageUrl
        {
            get => image;
            set
            {

                if (value != null)
                {
                    image = value;
                    LoadImage(value);
                }
            }
        }
        String AppData = Tools.CurrentSettings.CachePath;
        private string FilePath;
        public long OwnerID { get; set; }
        private String asyncImageProperty;
        public String AsyncImageProperty
        {
            get => asyncImageProperty;
            set
            {
                asyncImageProperty = value;
                OnPropertyChanged("AsyncImageProperty");
            }
        }
        private void LoadImage(string url)
        {
            Task.Run(() =>
            {
                FilePath = AppData + "\\" + "album" + ID.ToString();
                if (new FileInfo(FilePath).Exists)
                    AsyncImageProperty = FilePath;
                else
                {

                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                        try
                        {
                            webClient.DownloadFileAsync(new Uri(url), FilePath);
                        }

                        catch (Exception ex)
                        {
                            AsyncImageProperty = null;
                        }
                    }
                }
            });
        }
        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) =>
            AsyncImageProperty = FilePath;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }

}
