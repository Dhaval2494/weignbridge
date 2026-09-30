using log4net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.ViewModel.SettingViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for DatabaseSetting_Page.xaml
    /// </summary>
    public partial class ReceiptSetting_Page : Page
    {
        vmReceiptSetting viewModel;
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ReceiptSetting_Page()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmReceiptSetting(this);
            this.DataContext = viewModel;

            Mouse.OverrideCursor = null;
        }

        private void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Common.ValidatePageRequest(PageEntity.ReceiptSetting, PageAccess.Update))
            //    ReceiptSettingSave.Visibility = Visibility.Visible;
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            //viewModel.Dispose();
        }
    }
}
