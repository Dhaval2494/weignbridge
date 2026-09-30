using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    public partial class PluginManagement_Page : Page
    {
        vmPluginManagementList viewModel;
        public PluginManagement_Page()
        {
            viewModel = new vmPluginManagementList();
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
        }

        private void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.LoadPlugin();
            if (Common.ValidatePageRequest(PageEntity.PluginManagement, PageAccess.Update))
            {
                txtUploadPlugin.Visibility = Visibility.Visible;
                foreach (DataGridColumn col in DatagridPluginManagement.Columns)
                {
                    if (col != null && col.Header != null && col.Header.ToString() == "Action")
                        col.Visibility = Visibility.Visible;
                }
            }
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
        private void settingPopup_ClosingFinished(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            DataGridRow row = (DataGridRow)DatagridPluginManagement.ItemContainerGenerator.ContainerFromIndex(DatagridPluginManagement.SelectedIndex);
            row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
