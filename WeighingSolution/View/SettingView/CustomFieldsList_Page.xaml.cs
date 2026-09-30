using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for CustomFieldsList_Page.xaml
    /// </summary>
    public partial class CustomFieldsList_Page : Page
    {
        vmCustomFieldList viewModel;
        public CustomFieldsList_Page()
        {
            InitializeComponent();
            viewModel = new vmCustomFieldList(this);
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            this.DataContext = viewModel;
        }

        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            if (!Common.ValidatePageRequest(PageEntity.FieldSetting, PageAccess.Update))
            {
                btnCustomFieldSave.Visibility = Visibility.Collapsed;
                btnMasterFieldSave.Visibility = Visibility.Collapsed;
            }
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
