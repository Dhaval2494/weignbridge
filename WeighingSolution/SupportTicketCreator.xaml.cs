using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using System.Windows.Forms;
using WeighingSolution.ViewModel;

namespace WeighingSolution
{
    /// <summary>
    /// Interaction logic for SupportTicketCreator.xaml
    /// </summary>
    public partial class SupportTicketCreator : ChildWindow
    {
        vmSupportTicket viewModel;
        public SupportTicketCreator()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            viewModel = new vmSupportTicket(this);
            viewModel.lstAttachmentPath = new System.Collections.Generic.List<string>();
            this.DataContext = viewModel;
        }
        public SupportTicketCreator(string ScreenShotName)
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            viewModel = new vmSupportTicket(this);
            viewModel.ScreenShotName = ScreenShotName;
            viewModel.lstAttachmentPath = new System.Collections.Generic.List<string>();
            this.DataContext = viewModel;
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            if (fileDialog.ShowDialog() == DialogResult.OK && fileDialog.FileNames != null && fileDialog.FileNames.Length > 0)
            {
                string files = "";
                for (int i = 0; i < fileDialog.FileNames.Length; i++)
                {
                    files = string.IsNullOrEmpty(files) ? fileDialog.SafeFileNames[i] : files + ", " + fileDialog.SafeFileNames[i];
                    viewModel.lstAttachmentPath.Add(fileDialog.FileNames[i]);
                }
                lblSelectedFiles.Text = files;
            }
        }
    }
}
