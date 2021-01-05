using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace VkWpfPlayer.DataModels
{
    public class AudioModel
    {
        
        private string image;
        public long Owner_ID { get; set; }
        public int DurationSeconds { get; set; }
        public long ID { get; set; }
        public string Title { get; set; }
        public string AudioUrl { get; set; }
        public string AccessKey { get; set; }

        public string Artist { get; set; }
        public string ImageUrl
        {
            set
            {

                if (value != null)
                {
                    image = value;
                    LoadImage(value);
                }



            }
            get { return image; }
        }


        private string asyncImageProperty;
        private string FilePath;
        String AppData = ToolsAndsettings.CurrentSettings.CachePath;
        public String AsyncImageProperty
        {
            get => asyncImageProperty;
            set
            {
                asyncImageProperty = value;
                OnPropertyChanged("AsyncImageProperty");
            }
        }
        private async void LoadImage(string url)
        {
            _ = Task.Run(() =>
              {
                  FilePath = AppData + "\\" + "audio" + ID.ToString();
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
                              Console.WriteLine(ex.Message);



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
