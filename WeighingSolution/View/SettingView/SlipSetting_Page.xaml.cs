using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for SlipSetting_Page.xaml
    /// </summary>
    public partial class SlipSetting_Page : Page
    {
        vmSlipSetting viewModel;
        public SlipSetting_Page()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmSlipSetting(this);
            this.DataContext = viewModel;
            using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                lblLastTicketNo.Content = (ctx.Database.SqlQuery<decimal>("IF EXISTS (SELECT TOP 1 * FROM TransactionData) OR IDENT_CURRENT('transactiondata') <> 1 BEGIN SELECT IDENT_CURRENT('transactiondata') + 1 END ELSE BEGIN SELECT IDENT_CURRENT('transactiondata') END").FirstOrDefault()).ToString();
            }
        }

        private void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Common.ValidatePageRequest(PageEntity.GlobalSetting, PageAccess.Update))
                SlipSettingSave.Visibility = Visibility.Visible;
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

        private void btnResetTicketNo_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString()))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("IF EXISTS ( SELECT TOP 1 * FROM TransactionData) OR IDENT_CURRENT('transactiondata') > 1 BEGIN DBCC CHECKIDENT ('transactiondata', reseed, 0) END", con);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    ((MetroWindow)App.Current.MainWindow).ShowMessageAsync("Success", "Ticket # reset successfully.");
                }
                catch (Exception ex)
                {
                    con.Close();
                    throw ex;
                }
            }
            using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                lblLastTicketNo.Content = (ctx.Database.SqlQuery<decimal>("IF EXISTS (SELECT TOP 1 * FROM TransactionData) OR IDENT_CURRENT('transactiondata') <> 1 BEGIN SELECT IDENT_CURRENT('transactiondata') + 1 END ELSE BEGIN SELECT IDENT_CURRENT('transactiondata') END").FirstOrDefault()).ToString();
            }
        }
    }
}

