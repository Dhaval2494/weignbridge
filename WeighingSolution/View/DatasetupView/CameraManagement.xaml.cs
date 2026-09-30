using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    public partial class CameraManagement : Page
    {
        vmCameraManagement viewModel;
        public int IntCameraid = 0;

        #region CTOR
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmCameraManagement(IntCameraid);
            this.DataContext = viewModel;
            txtStatus.Text = "Ready";
        }

        public CameraManagement()
        {
            InitializeComponent();
            SetupPage();
        }

        public CameraManagement(int CameraID)
        {
            InitializeComponent();
            IntCameraid = CameraID;
            SetupPage();
        }
        #endregion
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Subscribe(GoBack);
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ipcam.vcVideoController != null)
            {
                ipcam.vcVideoController.StopVideo();
                ipcam.vcVideoController = null;
            }
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Unsubscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Unsubscribe(GoBack);
            viewModel.Dispose();
        }

        #region ClickEvent
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/CameraManagementList.xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private async void HandlePlayButtonClick(object sender, RoutedEventArgs e)
        {
            if (ipcam.vcVideoController == null)
            {
                ipcam.AutoPlay = true;
                await ipcam.InitializeVideoController();
            }
            if (ipcam.vcVideoController != null)
            {
                txtStatus.Text = "Connecting...";
                await ipcam.vcVideoController.StartVideo();
            }
        }

        private void HandleStopButtonClick(object sender, RoutedEventArgs e)
        {
            if (ipcam.vcVideoController != null)
            {
                txtStatus.Text = "Stopping...";
                ipcam.vcVideoController.StopVideo();
                ipcam.vcVideoController = null;
            }
        }

        private async void HandleImageButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "JPEG Image (.jpeg)|*.jpeg|JPG Image (.jpg)|*.jpg" };
            if (dialog.ShowDialog() == true)
            {
                await ipcam.vcVideoController.TakeSnapshot(dialog.FileName);
            }
        }

        private void UpdateButtons()
        {
            btnPlay.IsEnabled = !ipcam.IsPlaying;
            btnStop.IsEnabled = ipcam.IsPlaying;
        }

        private void HandlePlayerEvent(object sender, RoutedEventArgs e)
        {
            UpdateButtons();

            if (e.RoutedEvent.Name == "StreamStarted")
            {
                txtStatus.Text = "Playing";
                btnCaptureImage.IsEnabled = true;
            }
            else if (e.RoutedEvent.Name == "StreamFailed")
            {
                txtStatus.Text = "Failed";
                btnCaptureImage.IsEnabled = false;
            }

            else if (e.RoutedEvent.Name == "StreamStopped")
            {
                txtStatus.Text = "Stopped";
                btnCaptureImage.IsEnabled = false;
            }
        }
        #endregion
    }
}
