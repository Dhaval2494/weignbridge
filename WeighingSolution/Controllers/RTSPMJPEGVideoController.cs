using Meta.Vlc.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WeighingSolution;

namespace IPCamReader.Controllers
{
    public class RTSPMJPEGVideoController : VideoControllerBase
    {
        #region --- Fields ---
        public VlcPlayer vlcplayer;
        public Image player;
        private bool blnTakeSnapshot;
        private bool blnPauseVideo;
        private string strFilePath;
        #endregion

        #region --- Initialization ---

        public RTSPMJPEGVideoController(string _Url, string _IpAddress, ushort _Port, string _UserName, string _Password)
            : base(_Url, _IpAddress, _Port, _UserName, _Password)
        {
            player = new System.Windows.Controls.Image();
            vlcplayer = new VlcPlayer(player.Dispatcher);

            //vlcplayer.Initialize(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\LibVlc",
            //    new string[] 
            //    { 
            //        "-I",
            //        "--dummy-quiet", 
            //        "--ignore-config",
            //        "--no-video-title",
            //        "--no-sub-autodetect-file"
            //    });

            vlcplayer.Initialize(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\LibVlc",
                new string[]
                {
                        "-I",
                        "dummy",
                        "--ignore-config",
                        "--no-video-title",
                        "--no-sub-autodetect-file",
                        "--rtsp-tcp", "--network-caching=3000",
                        "--avcodec-hw=none",
                        "--live-caching=3000"
                });

            player.Source = vlcplayer.VideoSource;
            vlcplayer.VideoSourceChanged += (o, args) =>
            {
                try
                {
                    player.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        player.Source = args.NewVideoSource;
                        if (blnTakeSnapshot && args.NewVideoSource != null)
                        {
                            SaveImage(strFilePath);
                            blnTakeSnapshot = false;
                            if (blnPauseVideo)
                            {
                                PauseVideo();
                                blnPauseVideo = false;
                            }
                        }
                    }));
                }
                catch (Exception)
                {
                }
            };
            vlcplayer.StateChanged -= vlcplayer_StateChanged;
            vlcplayer.StateChanged += vlcplayer_StateChanged;

            VideoDisplay = player;
        }

        #endregion

        #region --- Methods ---
        public async override Task StartVideo()
        {
            try
            {
                await Task.Run(() =>
                {
                    vlcplayer.Stop();
                    vlcplayer.LoadMedia(CompleteURL);
                    vlcplayer.Play();
                });
            }
            catch (Exception)
            {
                RaiseVideoPlayFailed();
            }
        }
        public async override Task PauseVideo()
        {
            await Task.Run(() =>
            {
                vlcplayer.Pause();
            });
        }
        public async override Task StopVideo()
        {
            await Task.Run(() =>
            {
                vlcplayer.Stop();
            });
            RaiseVideoPlayStopped();
        }
        private void SaveImage(string FilePath)
        {
            try
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder() { QualityLevel = 70 };
                encoder.Frames.Add(BitmapFrame.Create(vlcplayer.VideoSource));
                using (FileStream fs = File.Create(FilePath))
                {
                    encoder.Save(fs);
                    fs.Flush(true);
                }
            }
            catch (Exception)
            {
            }
        }
        public override async Task TakeSnapshot(string FilePath)
        {
            try
            {
                if (vlcplayer.State == Meta.Vlc.Interop.Media.MediaState.Playing)
                    SaveImage(FilePath);
                else
                {
                    await StartVideo();
                    strFilePath = FilePath;
                    blnTakeSnapshot = true;
                    blnPauseVideo = true;

                    //Wait here until snapshot complete or for 5 secs
                    Task<bool> op = Task.Factory.StartNew<bool>(() =>
                    {
                        System.Threading.SpinWait.SpinUntil(() => blnTakeSnapshot == false, 5000);
                        return true;
                    });
                    await op;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override void TakePlateSnapshot(out string PlateNumber)
        {
            throw new NotImplementedException();
        }

        public override void TakePlateSnapshotWithNumber(string FilePath, out string PlateNumber)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region --- Events ---
        void vlcplayer_StateChanged(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            switch (e.Value)
            {
                case Meta.Vlc.Interop.Media.MediaState.Paused:
                    RaiseVideoPlayPaused();
                    break;
                case Meta.Vlc.Interop.Media.MediaState.Ended:
                case Meta.Vlc.Interop.Media.MediaState.Stopped:
                    RaiseVideoPlayStopped();
                    break;
                case Meta.Vlc.Interop.Media.MediaState.Playing:
                    RaiseVideoPlayStarted();
                    break;
                case Meta.Vlc.Interop.Media.MediaState.Error:
                    RaiseVideoPlayFailed();
                    break;
                default:
                    break;
            }
        }

       
        #endregion
    }

}
