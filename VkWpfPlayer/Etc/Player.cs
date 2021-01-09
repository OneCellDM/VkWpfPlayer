using ManagedBass;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using VkWpfPlayer.DataModels;

namespace VkWpfPlayer
{
    static class Player
    {
        private static bool Loading = false;
        private static Thread PlayThread;
        public static double volume = 1;
        public static bool IsRepeat = false;
        static bool stoopedHandled = false;
        public static bool IsPlaying = false;
        public static bool IsPaused = false;
        static private int _stream = 0;
        public delegate void ChannelActive();
        public static event ChannelActive Stopped;

        static private AudioModel audio;

        public delegate void AudioModelUpdate(AudioModel audioModel);
        public static event AudioModelUpdate updateAudioModel;

        static public AudioModel Audio { get => audio; set { audio = value; } }

        public delegate void PlayngPosition(int value);

        public static event PlayngPosition UpdatePosition;

        static System.Timers.Timer timer = new System.Timers.Timer();

        static Player()
        {
            volume = 1.0;
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            Tools.loggingHandler.Log.Info("Инициализация Bass");
            Bass.Configure(Configuration.IncludeDefaultDevice, 1);
            Bass.Init();

          


        }
        public static void SetPosition(double positionFromSeconds)
        {


            if (!Bass.ChannelSetPosition(_stream, Bass.ChannelSeconds2Bytes(_stream, positionFromSeconds)))
                Tools.loggingHandler.Log.Error(Bass.LastError);




        }
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {


            if (!stoopedHandled)
                if (Bass.ChannelIsActive(_stream) == PlaybackState.Stopped)
                {

                    if (IsRepeat)
                    {
                        Tools.loggingHandler.Log.Info("Перезапуск аудио потока");
                        Bass.ChannelUpdate(_stream, 0);
                        Player.Play();
                        if (IsPlaying)
                            Tools.loggingHandler.Log.Info("Аудио поток перезапущен");

                        else Tools.loggingHandler.Log.Error(Bass.LastError.ToString());
                    }
                    else
                    {
                        Tools.loggingHandler.Log.Info("Аудио поток остановлен");
                        if (Stopped != null)
                            Stopped.Invoke();
                        stoopedHandled = true;
                    }
                }


            if (UpdatePosition != null)
                UpdatePosition.Invoke(Convert.ToInt32(Bass.ChannelBytes2Seconds(_stream, Bass.ChannelGetPosition(_stream))));
        }

        static bool SetFromVkModel(AudioModel audioModel)
        {

            try
            {



                Audio = audioModel;
                if (updateAudioModel != null)
                    updateAudioModel.Invoke(audioModel);
                Audio.AudioUrl = Tools.VkApi.Audio.GetById(new String[] { Audio.Owner_ID.ToString() + "_" + Audio.ID + "_" + Audio.AccessKey }).ElementAt(0).Url.AbsoluteUri;
                SetFromUrl(Audio.AudioUrl);
                return true;
            }
            catch (Exception ex) { Tools.loggingHandler.Log.Error(ex); }

            return false;
        }


        public static void SetVolume(double value)
        {

            volume = value;
            if (!Bass.ChannelSetAttribute(_stream, ChannelAttribute.Volume, volume))
                Tools.loggingHandler.Log.Error(Bass.LastError);

        }
        static void SetFromUrl(String url)=>
            _stream = Bass.CreateStream(url, 0,  BassFlags.StreamStatus | BassFlags.AutoFree | BassFlags.Prescan | BassFlags.Unicode, null, IntPtr.Zero);
        
        public static async void Play(AudioModel audioModel, bool repeat = false)
        {

            if (Loading)
                return;


            timer.Stop();
            await Task.Run(() =>
            {

                Bass.ChannelStop(_stream);
                Bass.StreamFree(_stream);
                Loading = true;

                SetFromVkModel(audioModel);

            });
            Play();

        }
        public static void Pause()
        {
            if (Bass.ChannelPause(_stream))
            {
                Tools.loggingHandler.Log.Info("Аудио поток приостановлен");

                IsPaused = true;
                IsPlaying = false;
            }
            else
            {
                Tools.loggingHandler.Log.Info("Аудио поток не был приостановлен");
                Tools.loggingHandler.Log.Error(Bass.LastError);
            }
        }
       
        public static void Play()
        {


            
            if (PlayThread != null && PlayThread.IsAlive)
                PlayThread.Abort();

            if (_stream == 0)
            {
                Tools.loggingHandler.Log.Info("Длинна потока = 0");
                Loading = false;
            }
            else
            {


                PlayThread = new Thread(() =>
                  {
                      var reply = false;
                      if (!IsPaused)
                          reply = true;


                      if (!Bass.ChannelPlay(_stream, reply))
                      {
                          Tools.loggingHandler.Log.Info("Ошибка воспроизведения аудио потока");
                          Tools.loggingHandler.Log.Error(Bass.LastError);

                      }
                      SetVolume(volume);
                      timer.Start();
                      stoopedHandled = false;
                      IsPlaying = true;
                      IsPaused = false;
                      Loading = false;
                      Tools.loggingHandler.Log.Info("Аудио поток запущен");


                  });
                PlayThread.IsBackground = true;
                PlayThread.Priority = ThreadPriority.Lowest;
                PlayThread.Start();
            }

        }

    }
}
