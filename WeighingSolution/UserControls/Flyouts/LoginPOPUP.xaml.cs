using MahApps.Metro.Controls;
using Prism.Events;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;

namespace WeighingSolution.UserControls.Flyouts
{
    /// <summary>
    /// Interaction logic for Gate.xaml
    /// </summary>
    public partial class LoginPOPUP : UserControl
    {
        public weighbridgeEntities ctxGlobal = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString());
        public readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        public LoginPOPUP()
        {
            InitializeComponent();
        }

        private void CloseFlyout()
        {
            DependencyObject ucParent = this.Parent;
            while (!(ucParent is Flyout))
            {
                ucParent = LogicalTreeHelper.GetParent(ucParent);
            }
            if (ucParent != null)
                ((Flyout)ucParent).IsOpen = false;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            string username = txtusername.Text;
            string password = txtpassword.Password;
            password = Common.EncryptData(password);



            List<UserMaster> UserMasters = ctxGlobal.UserMasters.Where(x => x.UserName == username && x.Password == password).ToList();

            if (UserMasters.Count < 1)
            {
                ToastHelper.ShowError("Please enter correct admin credentials");
                return;
            }
            var data = UserMasters.FirstOrDefault();
            if (data.TemplateID == 1)
            {
                EventAggregator.GetEvent<AfterLoginEvent>().Publish(new AfterLoginEventargs
                {
                    TemplateID = data.TemplateID,
                    UserID = data.UserID,
                    UserName = data.UserName
                });
                CloseFlyout();
            }
            else
            {
                ToastHelper.ShowError("Please enter correct admin credentials");
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            CloseFlyout();

            //System.Windows.Application.Current.Shutdown();
        }
    }
}
