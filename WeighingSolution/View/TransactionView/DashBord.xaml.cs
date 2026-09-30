using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;
using WeighingSolution.UserControls;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for TransactionManagement.xaml
    /// </summary>
    public partial class DashBoard : Page
    {
        public DashboardViewModel viewModel;
        public Thread ComponentTest;


        #region CTOR + Window Function

        public DashBoard()
        {
            InitializeComponent();
            SetupPage();
        }
        public DashBoard(Guid ReceiptTicketID)
        {
            InitializeComponent();
            SetupPage();
        }
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;

            viewModel = new DashboardViewModel(this);
            this.DataContext = viewModel;
        }

        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComponentTest = new Thread(new ThreadStart(viewModel.SetHwComponent));
                ComponentTest.Start();
            }
            catch { }
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ComponentTest != null)
                {
                    ComponentTest.Abort();
                    ComponentTest = null;
                }
                viewModel.Dispose();
            }
            catch { }
        }
        
        #endregion


    }
}