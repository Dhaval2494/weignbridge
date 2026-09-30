using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using PluginInterfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WeighingSolution.Converters;
using WeighingSolution.DataLayer;
using WeighingSolution.SharedComponents;
using WeighingSolution.ViewModel;

namespace WeighingSolution.UserControls
{
    /// <summary>
    /// Interaction logic for TransactionReceiptPopup.xaml
    /// </summary>
    public partial class TransactionReceiptPopup : ChildWindow
    {
        private PluginLoader<IPluginControl> mef = new PluginLoader<IPluginControl>("TransactionReceiptPopup");
        DataTable childDT = new DataTable();
        DataTable childDTCloned = new DataTable();
        protected readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        private ContentControl _txPage;
        private VMTransactionReceiptPopup viewModel;
        public TransactionReceiptPopup()
        {
            InitializeComponent();
            viewModel = new VMTransactionReceiptPopup();
            this.DataContext = viewModel;

            SetupPlugin(this);
            try
            {
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.SetupPlugin(this);
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionReceiptPopupError.txt", json);
                //exec sp_updatestats
            }
            this.Loaded += TransactionReceiptPopup_Loaded;
            this.Unloaded += TransactionReceiptPopup_Unloaded;
        }

        void TransactionReceiptPopup_Loaded(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                TransactionListViewModel viewBindModel = this.DataContext as TransactionListViewModel;
                TransactionDataBindingModel objTranData = viewBindModel.SelectedTransaction as TransactionDataBindingModel;
                if (Common.ValidatePageRequest(PageEntity.PrintReceipt, PageAccess.Execute) && objTranData.TransactionDetails.Count >= 1 && (objTranData.TransactionDetails.FirstOrDefault().CaptureWeight != null && objTranData.TransactionDetails.FirstOrDefault().CaptureWeight.Value > 0))
                    btnPrintTransaction.Visibility = Visibility.Visible;

                var fields = viewModel.dataRepository.Context.CustomFields.Where(x => x.IsVisible).OrderBy(x => x.CustomFieldID).ToList();

                var list = new List<CustomFieldViewModel>();

                foreach (var field in fields)
                {
                    var vm = new CustomFieldViewModel
                    {
                        CustomFieldID = field.CustomFieldID,
                        DisplayName = field.DisplayName
                    };

                    // Map column: CustomFieldID=1 → CustomValue1
                    string columnName = $"CustomValue{field.CustomFieldID}";

                    var prop = viewBindModel.SelectedTransaction.GetType().GetProperty(columnName);

                    if (prop != null)
                        vm.CustomValue = prop.GetValue(viewBindModel.SelectedTransaction)?.ToString();

                    list.Add(vm);
                }
                viewBindModel.SelectedTransaction.CustomFieldViewModels = new ObservableCollection<CustomFieldViewModel>(list);

                viewModel.ViewTranscationImage(objTranData);
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionReportFilterPanelError.txt", json);
            }
            Mouse.OverrideCursor = null;

        }

        void TransactionReceiptPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.DisposePlugin();
                }
                viewModel.Dispose();
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionReceiptPopupError.txt", json);
                //exec sp_updatestats
            }
        }

        public void SetupPlugin(UIElement txPage)
        {
            try
            {
                _txPage = txPage as ContentControl;
                TabControl tabcntrl = _txPage.FindName("TabcntrlPopup") as TabControl;

                TabItem tabCamerareport = new TabItem
                {
                    Name = "tabpopupCapturedImages",
                    Header = "Captured Images",
                    Margin = new Thickness(0)
                };

                var capturedImagesControl = new CapturedImagesControl
                {
                    DataContext = viewModel // Or viewModel
                };

                tabCamerareport.Content = capturedImagesControl;

                Binding binding = new Binding("lstCamImage")
                {
                    Converter = new ListToVisibilityConverter(),
                    Source = viewModel,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(tabCamerareport, TabItem.VisibilityProperty, binding);

                tabcntrl.Items.Add(tabCamerareport);
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionReceiptPopupError.txt", json);
            }
        }

        private void btnPrintTransaction_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow parentwnd = ((MetroWindow)App.Current.MainWindow);
            Frame MainFrame = parentwnd.FindName("MainFrame") as Frame;
            TransactionListViewModel viewModel = this.DataContext as TransactionListViewModel;
            MainFrame.Content = new View.ReceiptPreview(viewModel.SelectedTransaction.ReceiptTicketID);
            this.Close();
        }

        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("Data", childDTCloned));
        }
    }
}
