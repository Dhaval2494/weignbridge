using IPCamReader.Controllers;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace WeighingSolution
{
    public enum CameraType
    {
        HikVision = 0,
        RTSPMJPEG = 1
    }
    public abstract class VideoControllerBase
    {
        #region --- Fields ---
        protected Uri CompleteURL;
        protected string Url;
        protected string IpAddress;
        protected ushort Port;
        protected string UserName;
        protected string Password;
        public UIElement VideoDisplay { get; set; }
        #endregion

        public VideoControllerBase(string _Url, string _IpAddress, ushort _Port, string _UserName, string _Password)
        {
            try
            {
                Url = _Url;
                IpAddress = _IpAddress;
                Port = _Port;
                UserName = _UserName;
                Password = _Password;
                if (!String.IsNullOrEmpty(Url))
                {
                    UriBuilder uriBuilder = new UriBuilder(Url);
                    uriBuilder.UserName = WebUtility.UrlEncode(UserName);
                    uriBuilder.Password = WebUtility.UrlEncode(Password);
                    CompleteURL = uriBuilder.Uri;
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async static Task<VideoControllerBase> CreateVideoController(CameraType protocolType, string url, string ipaddress, ushort port, string username, string password,
            EventHandler<RoutedEventArgs> RaiseStreamFailedEvent, EventHandler<RoutedEventArgs> RaiseStreamStartedEvent, EventHandler<RoutedEventArgs> RaiseStreamPausedEvent, EventHandler<RoutedEventArgs> RaiseStreamStoppedEvent,
            bool autoplay = true, System.Windows.Controls.Grid PlayerContainer = null, bool autoLoad = false)
        {
            VideoControllerBase vc = null;
            switch (protocolType)
            {
                case CameraType.HikVision:
                    vc = new HikVisionController(url, ipaddress, port, username, password);
                    break;
                case CameraType.RTSPMJPEG:
                    vc = new RTSPMJPEGVideoController(url, ipaddress, port, username, password);
                    break;
                default:
                    return null;
            }
            if (vc != null)
            {
                if (PlayerContainer != null)
                {
                    PlayerContainer.Children.Clear();
                    PlayerContainer.Children.Add(vc.VideoDisplay);
                }
                vc.VideoPlayFailed += RaiseStreamFailedEvent;
                vc.VideoPlayStarted += RaiseStreamStartedEvent;
                vc.VideoPlayPaused += RaiseStreamPausedEvent;
                vc.VideoPlayStopped += RaiseStreamStoppedEvent;

                if (autoplay)
                    await vc.StartVideo();
            }
            return vc;
        }

        #region --- Abstract Methods ---
        public abstract Task StartVideo();
        public abstract Task PauseVideo();
        public abstract Task StopVideo();
        public abstract Task TakeSnapshot(string FilePath);
        public abstract void TakePlateSnapshot(out string PlateNumber);
        public abstract void TakePlateSnapshotWithNumber(string FilePath, out string PlateNumber);
        #endregion

        #region --- Events ---
        public event EventHandler<RoutedEventArgs> VideoPlayStarted;
        public virtual void RaiseVideoPlayStarted()
        {
            if (VideoPlayStarted != null)
                VideoPlayStarted(this, new RoutedEventArgs());
        }

        public event EventHandler<RoutedEventArgs> VideoPlayPaused;
        public virtual void RaiseVideoPlayPaused()
        {
            if (VideoPlayPaused != null)
                VideoPlayPaused(this, new RoutedEventArgs());
        }

        public event EventHandler<RoutedEventArgs> VideoPlayStopped;
        public virtual void RaiseVideoPlayStopped()
        {
            if (VideoPlayStopped != null)
                VideoPlayStopped(this, new RoutedEventArgs());
        }

        public event EventHandler<RoutedEventArgs> VideoPlayFailed;
        public virtual void RaiseVideoPlayFailed()
        {
            if (VideoPlayFailed != null)
                VideoPlayFailed(this, new RoutedEventArgs());
        }
        #endregion
    }
}
