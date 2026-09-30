using System;
using System.ComponentModel;
using System.IO;
//using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WeighingSolution
{
    /// <summary>
    /// Interaction logic for CamViewer.xaml
    /// Interaction logic for CamViewer.xaml
    /// </summary>
    public partial class CamViewer : UserControl
    {

        public VideoControllerBase vcVideoController { get; set; }
        public Boolean AutoPlay { get; set; }


        private bool _autoLoad = false;
        public bool AutoLoad
        {
            get { return _autoLoad; }
            set { _autoLoad = value; }
        }


        // CameraId
        public static readonly DependencyProperty CameraIdProperty =
            DependencyProperty.Register("CameraId", typeof(int),
            typeof(CamViewer));
        [System.ComponentModel.Browsable(true)]
        public int CameraId
        {
            get { return (int)GetValue(CameraIdProperty); }
            set { SetValue(CameraIdProperty, value); }
        }

        //VideoFormat
        public static readonly DependencyProperty VideoFormatProperty =
            DependencyProperty.Register("VideoFormat", typeof(CameraType),
            typeof(CamViewer));
        [System.ComponentModel.Browsable(true)]
        public CameraType VideoFormat
        {
            get { return (CameraType)GetValue(VideoFormatProperty); }
            set { SetValue(VideoFormatProperty, value); }
        }

        //UserName
        public static readonly DependencyProperty UserNameProperty =
             DependencyProperty.Register("UserName", typeof(string),
             typeof(CamViewer));

        [System.ComponentModel.Browsable(true)]
        public String UserName
        {
            get { return (String)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        //Password
        public static readonly DependencyProperty PasswordProperty =
             DependencyProperty.Register("Password", typeof(string),
             typeof(CamViewer));

        [System.ComponentModel.Browsable(true)]
        public String Password
        {
            get { return (String)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        //IpAddress
        public static readonly DependencyProperty IpAddressProperty =
            DependencyProperty.Register("IpAddress", typeof(string),
            typeof(CamViewer));

        [System.ComponentModel.Browsable(true)]
        public String IpAddress
        {
            get { return (String)GetValue(IpAddressProperty); }
            set { SetValue(IpAddressProperty, value); }
        }

        //Port
        public static readonly DependencyProperty PortProperty =
            DependencyProperty.Register("Port", typeof(ushort),
            typeof(CamViewer));

        [System.ComponentModel.Browsable(true)]
        public ushort Port
        {
            get { return (ushort)GetValue(PortProperty); }
            set { SetValue(PortProperty, value); }
        }

        //VideoSourceURL
        public static readonly DependencyProperty VideoSourceURLProperty =
            DependencyProperty.Register("VideoSourceURL", typeof(string),
            typeof(CamViewer));

        [System.ComponentModel.Browsable(true)]
        public String VideoSourceURL
        {
            get { return (String)GetValue(VideoSourceURLProperty); }
            set { SetValue(VideoSourceURLProperty, value); }
        }

        //VideoSourceMainURL
        public static readonly DependencyProperty VideoSourceMainURLProperty =
            DependencyProperty.Register("VideoSourceMainURL", typeof(string),
            typeof(CamViewer));

        [System.ComponentModel.Browsable(true)]
        public String VideoSourceMainURL
        {
            get { return (String)GetValue(VideoSourceMainURLProperty); }
            set { SetValue(VideoSourceMainURLProperty, value); }
        }

        public CamViewer()
        {
            InitializeComponent();
            Loaded += CamViewer_Loaded;
            //Unloaded += CamViewer_Unloaded;
        }
        public async Task InitializeVideoController()
        {
            if (!String.IsNullOrEmpty(VideoSourceURL) || !String.IsNullOrEmpty(IpAddress))
            {
                if (vcVideoController != null)
                {
                    await vcVideoController.StopVideo();
                    vcVideoController = null;
                }

                vcVideoController = await VideoControllerBase.CreateVideoController(VideoFormat, VideoSourceURL, IpAddress, Port, UserName, Password, VideoPreviewFailed, VideoPreviewStarted, VideoPreviewPaused, VideoPreviewStopped, AutoPlay, VideoPreview);

            }
        }
        private async void CamViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            if (parent != null)
            {
                parent.Closed += parent_Closed;

                // https://stackoverflow.com/questions/3421303/loaded-event-of-a-wpf-user-control-fire-two-times
                Loaded -= CamViewer_Loaded;
            }
            if (AutoLoad)
            {
                DummyWindow dm = new DummyWindow();
                StackPanel stackPanel = new StackPanel { Orientation = Orientation.Vertical };
                stackPanel.Children.Add(this);
                dm.Content = stackPanel;
                dm.ShowDialog();
            }
            await InitializeVideoController();
        }

        void parent_Closed(object sender, EventArgs e)
        {
            if (vcVideoController != null)
            {
                if (IsPlaying)
                {
                    vcVideoController.StopVideo();
                    vcVideoController = null;
                }
            }

            Window window = sender as Window;
            if (window != null)
            {
                window.Closed -= parent_Closed;
            }
        }
        private void SetDefaultImage(string ImagePath)
        {
            Image imgDefault = new Image();
            imgDefault.Source = new BitmapImage(new Uri(ImagePath, UriKind.Relative));
            //imgDefault.Source = new ImageSourceConverter().ConvertFromString(ImagePath) as ImageSource;
            VideoPreview.Children.Clear();
            VideoPreview.Children.Add(imgDefault);
        }
        private void VideoPreviewStarted(object sender, RoutedEventArgs e)
        {
            IsPlaying = true;
            RaiseEvent(new RoutedEventArgs(StreamStartedEvent));
        }
        private void VideoPreviewPaused(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            RaiseEvent(new RoutedEventArgs(StreamPausedEvent));
        }
        private void VideoPreviewStopped(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            string path = Assembly.GetEntryAssembly().Location + @"\Images\DefaultCamera.jpg";
            SetDefaultImage(path);
            RaiseEvent(new RoutedEventArgs(StreamStoppedEvent));
        }
        public void VideoPreviewFailed(object sender, RoutedEventArgs e)
        {
            if (IsPlaying)
                vcVideoController.StopVideo();
            IsPlaying = false;
            string path = Assembly.GetEntryAssembly().Location + @"\Images\NoSignal.jpg";
            SetDefaultImage(path);
            vcVideoController = null;
            RaiseEvent(new RoutedEventArgs(StreamFailedEvent));
        }


        private static readonly DependencyPropertyKey IsPlayingPropertyKey =
            DependencyProperty.RegisterReadOnly("IsPlaying", typeof(Boolean), typeof(WeighingSolution.CamViewer),
        new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsPlayingProperty = IsPlayingPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating whether the control is playing a video stream.
        /// </summary>
        [Browsable(true)]
        public Boolean IsPlaying
        {
            get { return (Boolean)GetValue(IsPlayingProperty); }
            private set { SetValue(IsPlayingPropertyKey, value); }
        }

        public static readonly RoutedEvent StreamStartedEvent =
            EventManager.RegisterRoutedEvent("StreamStarted", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(WeighingSolution.CamViewer));
        /// <summary>
        /// Occurs when the first frame is read from a stream.
        /// </summary>
        public event RoutedEventHandler StreamStarted
        {
            add { AddHandler(StreamStartedEvent, value); }
            remove { RemoveHandler(StreamStartedEvent, value); }
        }

        public static readonly RoutedEvent StreamPausedEvent =
            EventManager.RegisterRoutedEvent("StreamPaused", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(WeighingSolution.CamViewer));
        /// <summary>
        /// Occurs when there are no more frames to read from a stream.
        /// </summary>
        public event RoutedEventHandler StreamPaused
        {
            add { AddHandler(StreamPausedEvent, value); }
            remove { RemoveHandler(StreamPausedEvent, value); }
        }

        public static readonly RoutedEvent StreamStoppedEvent =
            EventManager.RegisterRoutedEvent("StreamStopped", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(WeighingSolution.CamViewer));
        /// <summary>
        /// Occurs when there are no more frames to read from a stream.
        /// </summary>
        public event RoutedEventHandler StreamStopped
        {
            add { AddHandler(StreamStoppedEvent, value); }
            remove { RemoveHandler(StreamStoppedEvent, value); }
        }

        public static readonly RoutedEvent StreamFailedEvent =
            EventManager.RegisterRoutedEvent("StreamFailed", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(WeighingSolution.CamViewer));
        /// <summary>
        /// Occurs when the player fails to play a stream.
        /// </summary>
        public event RoutedEventHandler StreamFailed
        {
            add { AddHandler(StreamFailedEvent, value); }
            remove { RemoveHandler(StreamFailedEvent, value); }
        }
    }
}
