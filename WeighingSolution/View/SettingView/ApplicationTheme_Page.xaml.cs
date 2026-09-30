using System.Windows;
using System.Windows.Controls;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    public partial class ApplicationTheme_Page : Page
    {
        vmApplicationTheme viewModel;
        public ApplicationTheme_Page()
        {
            viewModel = new vmApplicationTheme(this);
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            //this.Loaded += CurrentPage_Loaded;
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}

