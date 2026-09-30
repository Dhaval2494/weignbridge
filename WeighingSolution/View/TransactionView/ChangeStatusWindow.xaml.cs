using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WeighingSolution.DataLayer;

namespace UnmannedPlugin
{
    public partial class ChangeStatusWindow : ChildWindow
    {

        TransactionTokenLog objTranTokenLog;
        public ChangeStatusWindow(TransactionTokenLog obj)
        {
            InitializeComponent();
            objTranTokenLog = obj as TransactionTokenLog;
            Cmbtokenstatus.SelectedIndex = 0;
        }

        private void btnChangeTokenstatus_Click(object sender, RoutedEventArgs e)
        {
            if (objTranTokenLog != null)
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
                {
                    //if token status Allocate, no change
                    TransactionTokenLog objTranLog = ctx.TransactionTokenLogs.FirstOrDefault(x => x.TransactionTokenLogID == objTranTokenLog.TransactionTokenLogID);
                    ComboBoxItem ComboItem = (ComboBoxItem)Cmbtokenstatus.SelectedItem;
                    if (ComboItem.Content.ToString() == "Free")
                    {
                        objTranLog.Token.TokenStatus = "Free";
                        objTranLog.FreeTime = DateTime.Now;
                        ctx.SaveChanges();
                    }
                }
                this.Close();
            }
        }
    }
}
