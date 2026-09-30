using log4net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for DatabaseSetting_Page.xaml
    /// </summary>
    public partial class PrintSetting_Page : Page
    {
        vmPrintSetting viewModel;
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public PrintSetting_Page()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmPrintSetting(this);
            this.DataContext = viewModel;

            Mouse.OverrideCursor = null;
        }

        private void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Common.ValidatePageRequest(PageEntity.PrintSetting, PageAccess.Update))
                PrintSettingSave.Visibility = Visibility.Visible;
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
