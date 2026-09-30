using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for GlobelSetting_Page.xaml
    /// </summary>
    public partial class GlobelSetting_Page : Page
    {
        vmGlobelSetting viewModel;
        public GlobelSetting_Page()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmGlobelSetting(this);
            this.DataContext = viewModel;
            //using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
            //{
            //    lblLastTicketNo.Content = (ctx.Database.SqlQuery<decimal>("IF EXISTS (SELECT TOP 1 * FROM TransactionData) OR IDENT_CURRENT('transactiondata') <> 1 BEGIN SELECT IDENT_CURRENT('transactiondata') + 1 END ELSE BEGIN SELECT IDENT_CURRENT('transactiondata') END").FirstOrDefault()).ToString();
            //}
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

        //private void btnResetTicketNo_Click(object sender, RoutedEventArgs e)
        //{
        //    using (SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString()))
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand("IF EXISTS ( SELECT TOP 1 * FROM TransactionData) OR IDENT_CURRENT('transactiondata') > 1 BEGIN DBCC CHECKIDENT ('transactiondata', reseed, 0) END", con);
        //            if (con.State != System.Data.ConnectionState.Open)
        //                con.Open();
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //            ((MetroWindow)App.Current.MainWindow).ShowMessageAsync("Success", "Ticket # reset successfully.");
        //        }
        //        catch (Exception ex)
        //        {
        //            con.Close();
        //            throw ex;
        //        }
        //    }
        //    using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
        //    {
        //        lblLastTicketNo.Content = (ctx.Database.SqlQuery<decimal>("IF EXISTS (SELECT TOP 1 * FROM TransactionData) OR IDENT_CURRENT('transactiondata') <> 1 BEGIN SELECT IDENT_CURRENT('transactiondata') + 1 END ELSE BEGIN SELECT IDENT_CURRENT('transactiondata') END").FirstOrDefault()).ToString();
        //    }
        //}
    }
}

