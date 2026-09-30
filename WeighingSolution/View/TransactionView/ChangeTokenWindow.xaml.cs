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
using UnmannedPlugin.ViewModel;

namespace UnmannedPlugin
{
    /// <summary>
    /// Interaction logic for ChangeTokenWindow.xaml
    /// </summary>
    public partial class ChangeTokenWindow : ChildWindow
    {
        vmChangeTokenWindow viewModel;
        Window window = Application.Current.MainWindow as Window;
        private Page Parentpage;
        private List<InputBinding> lstMwKeyBinding;
        private List<InputBinding> lstTranKeyBinding;
        public ChangeTokenWindow(Page Page)
        {
            InitializeComponent();
            Parentpage = Page;
            this.Loaded += ChangeTokenWindow_Loaded;
            this.Unloaded += CurrentPage_Unloaded;
            viewModel = new vmChangeTokenWindow(this);
            this.DataContext = viewModel;
        }

        void ChangeTokenWindow_Loaded(object sender, RoutedEventArgs e)
        {

            if (window.InputBindings != null)
            {
                lstMwKeyBinding = new List<InputBinding>();
                foreach (InputBinding ib in window.InputBindings)
                {
                    lstMwKeyBinding.Add(ib);
                }
                window.InputBindings.Clear();
            }
            if (Parentpage != null && Parentpage.InputBindings != null)
            {
                lstTranKeyBinding = new List<InputBinding>();
                foreach (InputBinding ib in Parentpage.InputBindings)
                {
                    lstTranKeyBinding.Add(ib);
                }
                Parentpage.InputBindings.Clear();
            }
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (lstMwKeyBinding != null && lstMwKeyBinding.Count > 0)
            {
                foreach (InputBinding ib in lstMwKeyBinding)
                {
                    window.InputBindings.Add(ib);
                }
            }
            if (lstTranKeyBinding != null && lstTranKeyBinding.Count > 0)
            {
                foreach (InputBinding ib in lstTranKeyBinding)
                {
                    Parentpage.InputBindings.Add(ib);
                }
            }
            viewModel.Dispose();
        }
    }
}
