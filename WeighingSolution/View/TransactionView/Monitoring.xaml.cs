using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    public partial class Monitoring : Page
    {
      

        #region CTOR + Window Function
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
           
        }
        public Monitoring()
        {
            InitializeComponent();
            SetupPage();
        }
     
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            Constant.ISManual = true;
        }
       

       

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Constant.ISManual = false;
            MainWindow mw = (App.Current.MainWindow) as MainWindow;
            if (mw != null)
            {
                Flyout flyoutContainer = (Flyout)mw.FindName("flyoutContainer");
                Flyout flyChangePassword = (Flyout)mw.FindName("flyChangePassword");
                flyChangePassword.IsOpen = false;
                flyoutContainer.IsOpen = false;

                //Remove Key Bindings from top window.
                foreach (InputBinding ib in this.InputBindings)
                {
                    mw.InputBindings.Remove(ib);
                }
            }
           
        }
        #endregion


        private void PmonitorCancel_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/TransactionView/TransactionManagement.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

       
        private void lefttboomclose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.modbusHelper.Boom_Left_Close();
        }

        private void lefgreen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.modbusHelper.Trafiic_Left_Green();
        }

        private void leftred_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.modbusHelper.Trafiic_Left_Red();
        }

        private void Leftboomopen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.modbusHelper.Boom_Left_Open();
        }

        private void rightboomopen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.modbusHelper.Boom_Right_Open();
        }

        private void rightboomclose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.modbusHelper.Boom_Right_Close();
        }

        private void rightgreen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.modbusHelper.Trafiic_Right_Green();
        }

        private void rightred_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.modbusHelper.Trafiic_Right_Red();
        }
    }
}