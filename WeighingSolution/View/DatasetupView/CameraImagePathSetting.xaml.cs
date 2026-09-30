using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using WeighingSolution.ViewModel;

namespace WeighingSolution
{
    /// <summary>
    /// Interaction logic for CameraImagePathSetting.xaml
    /// </summary>
    public partial class CameraImagePathSetting : ChildWindow
    {
        public vmCameraImagePathSetting viewModel;
        public CameraImagePathSetting()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            viewModel = new vmCameraImagePathSetting(this);
            this.DataContext = viewModel;
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

    }
}
