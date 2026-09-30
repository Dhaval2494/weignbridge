using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    public partial class SMSSchedulePage : Page
    {
        vmSMSSchedulePage viewModel;
        public int IntSMSsetupid = 0;

        #region CTOR
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmSMSSchedulePage(this, IntSMSsetupid);
            this.DataContext = viewModel;
            using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                List<CustomField> lstcstmfield = ctx.CustomFields.ToList();
                lblField1.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field1").DisplayName;
                lblField2.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field2").DisplayName;
                lblField3.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field3").DisplayName;
                lblField4.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field4").DisplayName;
                lblField5.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field5").DisplayName;
                lblField6.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field6").DisplayName;
                lblField7.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field7").DisplayName;
                lblField8.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field8").DisplayName;
                lblField9.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field9").DisplayName;
                lblField10.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field10").DisplayName;
                lblField11.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field11").DisplayName;
                lblField12.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field12").DisplayName;
                lblField13.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field13").DisplayName;
                lblField14.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field14").DisplayName;
                lblField15.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field15").DisplayName;
                lblField16.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field16").DisplayName;
                lblField17.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field17").DisplayName;
                lblField18.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field18").DisplayName;
                lblField19.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field19").DisplayName;
                lblField20.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == "Field20").DisplayName;
            }
            using (weighbridgeEntities ctxSMSSetting = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                List<SMSFieldSetting> lstSMSSetting = ctxSMSSetting.SMSFieldSettings.Where(p => p.SMSSetupId == IntSMSsetupid).ToList();
                if (lstSMSSetting != null && lstSMSSetting.Count > 0)
                {
                    SMSFieldSetting objTicketNo = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Ticket No");
                    if (objTicketNo != null)
                    {
                        cbTicketNo.IsChecked = objTicketNo.IsSelected;
                        txtTicketNo.Text = objTicketNo.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objVehicleNo = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Vehicle No");
                    if (objVehicleNo != null)
                    {
                        cbVehicleNo.IsChecked = objVehicleNo.IsSelected;
                        txtVehicleNo.Text = objVehicleNo.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objAccountName = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Account Name");
                    if (objAccountName != null)
                    {
                        cbAccountName.IsChecked = objAccountName.IsSelected;
                        txtAccountName.Text = objAccountName.SMSDisplayName.Trim();
                    }

                    //BarcodeSetting objTokenValue = lstBarcode.FirstOrDefault(x => x.BarcodeFieldName == "Token Value");
                    //if (objTokenValue != null)
                    //{
                    //    cbTokenValue.IsChecked = objTokenValue.IsSelected;
                    //    txtTokenValue.Text = objTokenValue.BarcodeDisplayName.Trim();
                    //}

                    SMSFieldSetting objGrossWeight = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Gross Weight");
                    if (objGrossWeight != null)
                    {
                        cbGrossWeight.IsChecked = objGrossWeight.IsSelected;
                        txtGrossWeight.Text = objGrossWeight.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objGrossTime = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Gross Time");
                    if (objGrossTime != null)
                    {
                        cbGrossTime.IsChecked = objGrossTime.IsSelected;
                        txtGrossTime.Text = objGrossTime.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objTareWeight = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Tare Weight");
                    if (objTareWeight != null)
                    {
                        cbTareWeight.IsChecked = objTareWeight.IsSelected;
                        txtTareWeight.Text = objTareWeight.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objTareTime = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Tare Time");
                    if (objTareTime != null)
                    {
                        cbTareTime.IsChecked = objTareTime.IsSelected;
                        txtTareTime.Text = objTareTime.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objNetWeight = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Net Weight");
                    if (objNetWeight != null)
                    {
                        cbNetWeight.IsChecked = objNetWeight.IsSelected;
                        txtNetWeight.Text = objNetWeight.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objCharges = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Charges");
                    if (objCharges != null)
                    {
                        cbCharges.IsChecked = objCharges.IsSelected;
                        txtCharges.Text = objCharges.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objProduct = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Product");
                    if (objProduct != null)
                    {
                        cbProduct.IsChecked = objProduct.IsSelected;
                        txtProduct.Text = objProduct.SMSDisplayName.Trim();
                    }


                    SMSFieldSetting objField1 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field1");
                    if (objField1 != null)
                    {
                        cbField1.IsChecked = objField1.IsSelected;
                        txtField1.Text = objField1.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField2 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field2");
                    if (objField2 != null)
                    {
                        cbField2.IsChecked = objField2.IsSelected;
                        txtField2.Text = objField2.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField3 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field3");
                    if (objField3 != null)
                    {
                        cbField3.IsChecked = objField3.IsSelected;
                        txtField3.Text = objField3.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField4 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field4");
                    if (objField4 != null)
                    {
                        cbField4.IsChecked = objField4.IsSelected;
                        txtField4.Text = objField4.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField5 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field5");
                    if (objField5 != null)
                    {
                        cbField5.IsChecked = objField5.IsSelected;
                        txtField5.Text = objField5.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField6 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field6");
                    if (objField6 != null)
                    {
                        cbField6.IsChecked = objField6.IsSelected;
                        txtField6.Text = objField6.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField7 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field7");
                    if (objField7 != null)
                    {
                        cbField7.IsChecked = objField7.IsSelected;
                        txtField7.Text = objField7.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField8 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field8");
                    if (objField8 != null)
                    {
                        cbField8.IsChecked = objField8.IsSelected;
                        txtField8.Text = objField8.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField9 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field9");
                    if (objField9 != null)
                    {
                        cbField9.IsChecked = objField9.IsSelected;
                        txtField9.Text = objField9.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField10 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field10");
                    if (objField10 != null)
                    {
                        cbField10.IsChecked = objField10.IsSelected;
                        txtField10.Text = objField10.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField11 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field11");
                    if (objField11 != null)
                    {
                        cbField11.IsChecked = objField11.IsSelected;
                        txtField11.Text = objField11.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField12 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field12");
                    if (objField12 != null)
                    {
                        cbField12.IsChecked = objField12.IsSelected;
                        txtField12.Text = objField12.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField13 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field13");
                    if (objField13 != null)
                    {
                        cbField13.IsChecked = objField13.IsSelected;
                        txtField13.Text = objField13.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField14 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field14");
                    if (objField14 != null)
                    {
                        cbField14.IsChecked = objField14.IsSelected;
                        txtField14.Text = objField14.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField15 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field15");
                    if (objField15 != null)
                    {
                        cbField15.IsChecked = objField15.IsSelected;
                        txtField15.Text = objField15.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField16 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field16");
                    if (objField16 != null)
                    {
                        cbField16.IsChecked = objField16.IsSelected;
                        txtField16.Text = objField16.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField17 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field17");
                    if (objField17 != null)
                    {
                        cbField17.IsChecked = objField17.IsSelected;
                        txtField17.Text = objField17.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField18 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field18");
                    if (objField18 != null)
                    {
                        cbField18.IsChecked = objField18.IsSelected;
                        txtField18.Text = objField18.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField19 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field19");
                    if (objField19 != null)
                    {
                        cbField19.IsChecked = objField19.IsSelected;
                        txtField19.Text = objField19.SMSDisplayName.Trim();
                    }

                    SMSFieldSetting objField20 = lstSMSSetting.FirstOrDefault(x => x.SMSFieldName == "Field20");
                    if (objField20 != null)
                    {
                        cbField20.IsChecked = objField20.IsSelected;
                        txtField20.Text = objField20.SMSDisplayName.Trim();
                    }
                }
            }
        }

        public SMSSchedulePage()
        {
            InitializeComponent();
            SetupPage();
        }
        public SMSSchedulePage(int SMSSetupID)
        {
            InitializeComponent();
            IntSMSsetupid = SMSSetupID;
            SetupPage();
        }
        #endregion
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

        private async void SMSSeduleSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bool isSMSToRequired = cmbConnectionType.Text != "Every Transaction" ? true : !string.IsNullOrEmpty(txtSMSTo.Text);
                if (!string.IsNullOrEmpty(txtScheduleName.Text) && isSMSToRequired && !string.IsNullOrEmpty(cmbConnectionType.Text))
                {
                    if (IsValidate())
                    {
                        viewModel.SaveData();

                        IntSMSsetupid = viewModel.Model.SMSSetupID;

                        using (weighbridgeEntities ctxBarcode = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
                        {
                            List<SMSFieldSetting> lstSMSField = ctxBarcode.SMSFieldSettings.ToList();
                            SMSFieldSetting objBarCode = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Ticket No");
                            if (objBarCode == null)
                            {
                                objBarCode = new SMSFieldSetting();
                                objBarCode.SMSSetupId = IntSMSsetupid;
                                objBarCode.SMSFieldName = lblTicketNo.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objBarCode);
                            }
                            objBarCode.SMSDisplayName = txtTicketNo.Text;
                            objBarCode.IsSelected = cbTicketNo.IsChecked.GetValueOrDefault(false);

                            SMSFieldSetting objVehicleNo = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Vehicle No");
                            if (objVehicleNo == null)
                            {
                                objVehicleNo = new SMSFieldSetting();
                                objVehicleNo.SMSSetupId = IntSMSsetupid;
                                objVehicleNo.SMSFieldName = lblVehicleNo.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objVehicleNo);
                            }
                            objVehicleNo.IsSelected = cbVehicleNo.IsChecked.GetValueOrDefault(false);
                            objVehicleNo.SMSDisplayName = txtVehicleNo.Text;

                            SMSFieldSetting objAccountName = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Account Name");
                            if (objAccountName == null)
                            {
                                objAccountName = new SMSFieldSetting();
                                objAccountName.SMSSetupId = IntSMSsetupid;
                                objAccountName.SMSFieldName = lblAccountName.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objAccountName);
                            }
                            objAccountName.IsSelected = cbAccountName.IsChecked.GetValueOrDefault(false);
                            objAccountName.SMSDisplayName = txtAccountName.Text.Trim();

                            //BarcodeSetting objTokenValue = lstBarcode.FirstOrDefault(x => x.BarcodeFieldName == "Token Value");
                            //if (objTokenValue == null)
                            //{
                            //    objTokenValue = new BarcodeSetting();
                            //    objTokenValue.BarcodeFieldName = lblTokenValue.Content.ToString();
                            //    ctxBarcode.BarcodeSettings.Add(objTokenValue);
                            //}
                            //objTokenValue.IsSelected = cbTokenValue.IsChecked.GetValueOrDefault(false);
                            //objTokenValue.BarcodeDisplayName = txtTokenValue.Text.Trim();

                            SMSFieldSetting objGrossWeight = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Gross Weight");
                            if (objGrossWeight == null)
                            {
                                objGrossWeight = new SMSFieldSetting();
                                objGrossWeight.SMSSetupId = IntSMSsetupid;
                                objGrossWeight.SMSFieldName = lblGrossWeight.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objGrossWeight);
                            }
                            objGrossWeight.IsSelected = cbGrossWeight.IsChecked.GetValueOrDefault(false);
                            objGrossWeight.SMSDisplayName = txtGrossWeight.Text.Trim();

                            SMSFieldSetting objGrossTime = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Gross Time");
                            if (objGrossTime == null)
                            {
                                objGrossTime = new SMSFieldSetting();
                                objGrossTime.SMSSetupId = IntSMSsetupid;
                                objGrossTime.SMSFieldName = lblGrossTime.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objGrossTime);
                            }
                            objGrossTime.IsSelected = cbGrossTime.IsChecked.GetValueOrDefault(false);
                            objGrossTime.SMSDisplayName = txtGrossTime.Text.Trim();

                            SMSFieldSetting objTareWeight = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Tare Weight");
                            if (objTareWeight == null)
                            {
                                objTareWeight = new SMSFieldSetting();
                                objTareWeight.SMSSetupId = IntSMSsetupid;
                                objTareWeight.SMSFieldName = lblTareWeight.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objTareWeight);
                            }
                            objTareWeight.IsSelected = cbTareWeight.IsChecked.GetValueOrDefault(false);
                            objTareWeight.SMSDisplayName = txtTareWeight.Text.Trim();

                            SMSFieldSetting objTareTime = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Tare Time");
                            if (objTareTime == null)
                            {
                                objTareTime = new SMSFieldSetting();
                                objTareTime.SMSSetupId = IntSMSsetupid;
                                objTareTime.SMSFieldName = lblTareTime.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objTareTime);
                            }
                            objTareTime.IsSelected = cbTareTime.IsChecked.GetValueOrDefault(false);
                            objTareTime.SMSDisplayName = txtTareTime.Text.Trim();

                            SMSFieldSetting objNetWeight = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Net Weight");
                            if (objNetWeight == null)
                            {
                                objNetWeight = new SMSFieldSetting();
                                objNetWeight.SMSSetupId = IntSMSsetupid;
                                objNetWeight.SMSFieldName = lblNetWeight.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objNetWeight);
                            }
                            objNetWeight.IsSelected = cbNetWeight.IsChecked.GetValueOrDefault(false);
                            objNetWeight.SMSDisplayName = txtNetWeight.Text.Trim();


                            SMSFieldSetting objProduct = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Product");
                            if (objProduct == null)
                            {
                                objProduct = new SMSFieldSetting();
                                objProduct.SMSSetupId = IntSMSsetupid;
                                objProduct.SMSFieldName = lblProduct.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objProduct);
                            }
                            objProduct.IsSelected = cbProduct.IsChecked.GetValueOrDefault(false);
                            objProduct.SMSDisplayName = txtProduct.Text.Trim();

                            SMSFieldSetting objCharges = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Charges");
                            if (objCharges == null)
                            {
                                objCharges = new SMSFieldSetting();
                                objCharges.SMSSetupId = IntSMSsetupid;
                                objCharges.SMSFieldName = lblCharges.Content.ToString();
                                ctxBarcode.SMSFieldSettings.Add(objCharges);
                            }
                            objCharges.IsSelected = cbCharges.IsChecked.GetValueOrDefault(false);
                            objCharges.SMSDisplayName = txtCharges.Text.Trim();


                            SMSFieldSetting objField1 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field1");
                            if (objField1 == null)
                            {
                                objField1 = new SMSFieldSetting();
                                objField1.SMSSetupId = IntSMSsetupid;
                                objField1.SMSFieldName = "Field1";
                                ctxBarcode.SMSFieldSettings.Add(objField1);
                            }
                            objField1.IsSelected = cbField1.IsChecked.GetValueOrDefault(false);
                            objField1.SMSDisplayName = txtField1.Text.Trim();


                            SMSFieldSetting objField2 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field2");
                            if (objField2 == null)
                            {
                                objField2 = new SMSFieldSetting();
                                objField2.SMSSetupId = IntSMSsetupid;
                                objField2.SMSFieldName = "Field2";
                                ctxBarcode.SMSFieldSettings.Add(objField2);
                            }
                            objField2.IsSelected = cbField2.IsChecked.GetValueOrDefault(false);
                            objField2.SMSDisplayName = txtField2.Text.Trim();


                            SMSFieldSetting objField3 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field3");
                            if (objField3 == null)
                            {
                                objField3 = new SMSFieldSetting();
                                objField3.SMSSetupId = IntSMSsetupid;
                                objField3.SMSFieldName = "Field3";
                                ctxBarcode.SMSFieldSettings.Add(objField3);
                            }
                            objField3.IsSelected = cbField3.IsChecked.GetValueOrDefault(false);
                            objField3.SMSDisplayName = txtField3.Text.Trim();


                            SMSFieldSetting objField4 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field4");
                            if (objField4 == null)
                            {
                                objField4 = new SMSFieldSetting();
                                objField4.SMSSetupId = IntSMSsetupid;
                                objField4.SMSFieldName = "Field4";
                                ctxBarcode.SMSFieldSettings.Add(objField4);
                            }
                            objField4.IsSelected = cbField4.IsChecked.GetValueOrDefault(false);
                            objField4.SMSDisplayName = txtField4.Text.Trim();


                            SMSFieldSetting objField5 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field5");
                            if (objField5 == null)
                            {
                                objField5 = new SMSFieldSetting();
                                objField5.SMSSetupId = IntSMSsetupid;
                                objField5.SMSFieldName = "Field5";
                                ctxBarcode.SMSFieldSettings.Add(objField5);
                            }
                            objField5.IsSelected = cbField5.IsChecked.GetValueOrDefault(false);
                            objField5.SMSDisplayName = txtField5.Text.Trim();


                            SMSFieldSetting objField6 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field6");
                            if (objField6 == null)
                            {
                                objField6 = new SMSFieldSetting();
                                objField6.SMSSetupId = IntSMSsetupid;
                                objField6.SMSFieldName = "Field6";
                                ctxBarcode.SMSFieldSettings.Add(objField6);
                            }
                            objField6.IsSelected = cbField6.IsChecked.GetValueOrDefault(false);
                            objField6.SMSDisplayName = txtField6.Text.Trim();


                            SMSFieldSetting objField7 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field7");
                            if (objField7 == null)
                            {
                                objField7 = new SMSFieldSetting();
                                objField7.SMSSetupId = IntSMSsetupid;
                                objField7.SMSFieldName = "Field7";
                                ctxBarcode.SMSFieldSettings.Add(objField7);
                            }
                            objField7.IsSelected = cbField7.IsChecked.GetValueOrDefault(false);
                            objField7.SMSDisplayName = txtField7.Text.Trim();


                            SMSFieldSetting objField8 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field8");
                            if (objField8 == null)
                            {
                                objField8 = new SMSFieldSetting();
                                objField8.SMSSetupId = IntSMSsetupid;
                                objField8.SMSFieldName = "Field8";
                                ctxBarcode.SMSFieldSettings.Add(objField8);
                            }
                            objField8.IsSelected = cbField8.IsChecked.GetValueOrDefault(false);
                            objField8.SMSDisplayName = txtField8.Text.Trim();

                            SMSFieldSetting objField9 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field9");
                            if (objField9 == null)
                            {
                                objField9 = new SMSFieldSetting();
                                objField9.SMSSetupId = IntSMSsetupid;
                                objField9.SMSFieldName = "Field9";
                                ctxBarcode.SMSFieldSettings.Add(objField9);
                            }
                            objField9.IsSelected = cbField9.IsChecked.GetValueOrDefault(false);
                            objField9.SMSDisplayName = txtField9.Text.Trim();

                            SMSFieldSetting objField10 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field10");
                            if (objField10 == null)
                            {
                                objField10 = new SMSFieldSetting();
                                objField10.SMSSetupId = IntSMSsetupid;
                                objField10.SMSFieldName = "Field10";
                                ctxBarcode.SMSFieldSettings.Add(objField10);
                            }
                            objField10.IsSelected = cbField10.IsChecked.GetValueOrDefault(false);
                            objField10.SMSDisplayName = txtField10.Text.Trim();

                            SMSFieldSetting objField11 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field11");
                            if (objField11 == null)
                            {
                                objField11 = new SMSFieldSetting();
                                objField11.SMSSetupId = IntSMSsetupid;
                                objField11.SMSFieldName = "Field11";
                                ctxBarcode.SMSFieldSettings.Add(objField11);
                            }
                            objField11.IsSelected = cbField11.IsChecked.GetValueOrDefault(false);
                            objField11.SMSDisplayName = txtField11.Text.Trim();

                            SMSFieldSetting objField12 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field12");
                            if (objField12 == null)
                            {
                                objField12 = new SMSFieldSetting();
                                objField12.SMSSetupId = IntSMSsetupid;
                                objField12.SMSFieldName = "Field12";
                                ctxBarcode.SMSFieldSettings.Add(objField12);
                            }
                            objField12.IsSelected = cbField12.IsChecked.GetValueOrDefault(false);
                            objField12.SMSDisplayName = txtField12.Text.Trim();

                            SMSFieldSetting objField13 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field13");
                            if (objField13 == null)
                            {
                                objField13 = new SMSFieldSetting();
                                objField13.SMSSetupId = IntSMSsetupid;
                                objField13.SMSFieldName = "Field13";
                                ctxBarcode.SMSFieldSettings.Add(objField13);
                            }
                            objField13.IsSelected = cbField13.IsChecked.GetValueOrDefault(false);
                            objField13.SMSDisplayName = txtField13.Text.Trim();

                            SMSFieldSetting objField14 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field14");
                            if (objField14 == null)
                            {
                                objField14 = new SMSFieldSetting();
                                objField14.SMSSetupId = IntSMSsetupid;
                                objField14.SMSFieldName = "Field14";
                                ctxBarcode.SMSFieldSettings.Add(objField14);
                            }
                            objField14.IsSelected = cbField14.IsChecked.GetValueOrDefault(false);
                            objField14.SMSDisplayName = txtField14.Text.Trim();

                            SMSFieldSetting objField15 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field15");
                            if (objField15 == null)
                            {
                                objField15 = new SMSFieldSetting();
                                objField15.SMSSetupId = IntSMSsetupid;
                                objField15.SMSFieldName = "Field15";
                                ctxBarcode.SMSFieldSettings.Add(objField15);
                            }
                            objField15.IsSelected = cbField15.IsChecked.GetValueOrDefault(false);
                            objField15.SMSDisplayName = txtField15.Text.Trim();

                            SMSFieldSetting objField16 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field16");
                            if (objField16 == null)
                            {
                                objField16 = new SMSFieldSetting();
                                objField16.SMSSetupId = IntSMSsetupid;
                                objField16.SMSFieldName = "Field16";
                                ctxBarcode.SMSFieldSettings.Add(objField16);
                            }
                            objField16.IsSelected = cbField16.IsChecked.GetValueOrDefault(false);
                            objField16.SMSDisplayName = txtField16.Text.Trim();

                            SMSFieldSetting objField17 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field17");
                            if (objField17 == null)
                            {
                                objField17 = new SMSFieldSetting();
                                objField17.SMSSetupId = IntSMSsetupid;
                                objField17.SMSFieldName = "Field17";
                                ctxBarcode.SMSFieldSettings.Add(objField17);
                            }
                            objField17.IsSelected = cbField17.IsChecked.GetValueOrDefault(false);
                            objField17.SMSDisplayName = txtField17.Text.Trim();

                            SMSFieldSetting objField18 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field18");
                            if (objField18 == null)
                            {
                                objField18 = new SMSFieldSetting();
                                objField18.SMSSetupId = IntSMSsetupid;
                                objField18.SMSFieldName = "Field18";
                                ctxBarcode.SMSFieldSettings.Add(objField18);
                            }
                            objField18.IsSelected = cbField18.IsChecked.GetValueOrDefault(false);
                            objField18.SMSDisplayName = txtField18.Text.Trim();

                            SMSFieldSetting objField19 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field19");
                            if (objField19 == null)
                            {
                                objField19 = new SMSFieldSetting();
                                objField19.SMSSetupId = IntSMSsetupid;
                                objField19.SMSFieldName = "Field19";
                                ctxBarcode.SMSFieldSettings.Add(objField19);
                            }
                            objField19.IsSelected = cbField19.IsChecked.GetValueOrDefault(false);
                            objField19.SMSDisplayName = txtField19.Text.Trim();

                            SMSFieldSetting objField20 = lstSMSField.Where(x => x.SMSSetupId == IntSMSsetupid).FirstOrDefault(x => x.SMSFieldName == "Field20");
                            if (objField20 == null)
                            {
                                objField20 = new SMSFieldSetting();
                                objField20.SMSSetupId = IntSMSsetupid;
                                objField20.SMSFieldName = "Field20";
                                ctxBarcode.SMSFieldSettings.Add(objField20);
                            }
                            objField20.IsSelected = cbField20.IsChecked.GetValueOrDefault(false);
                            objField20.SMSDisplayName = txtField20.Text.Trim();

                            ctxBarcode.SaveChanges();
                            WeighingSolution.Helpers.ToastHelper.ShowSuccess("SMS Template saved successfully");
                            Uri uri = new Uri("/View/SettingView/SMSScheduleList_Page.Xaml", UriKind.Relative);
                            this.NavigationService.Navigate(uri);
                        }
                    }
                }
                else
                {
                    await MessageBoxHelper.ShowInfoDialogAsync("SMS Setting", "Please fill all the required details");
                    return;
                }
            }
            catch (Exception ex) { }
        }
        private bool IsValidate()
        {
            bool Isfocus = false;
            string ErrorMsg = "";
            if (cbTicketNo.IsChecked == true && string.IsNullOrWhiteSpace(txtTicketNo.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtTicketNo);
                }
                ErrorMsg = "Please insert display name of ticket no" + "\n";
            }
            if (cbVehicleNo.IsChecked == true && string.IsNullOrWhiteSpace(txtVehicleNo.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtVehicleNo);
                }
                ErrorMsg += "Please insert display name of vehicle no" + "\n";
            }
            if (cbAccountName.IsChecked == true && string.IsNullOrWhiteSpace(txtAccountName.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtAccountName);
                }
                ErrorMsg += "Please insert display name of account name" + "\n";
            }
            //if (cbTokenValue.IsChecked == true && string.IsNullOrWhiteSpace(txtTokenValue.Text))
            //{
            //    if (!Isfocus)
            //    {
            //        Isfocus = true;
            //        Keyboard.Focus(txtTokenValue);
            //    }
            //    ErrorMsg += "Please insert display name of token value" + "\n";
            //}
            if (cbGrossWeight.IsChecked == true && string.IsNullOrWhiteSpace(txtGrossWeight.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtGrossWeight);
                }
                ErrorMsg += "Please insert display name of gross weight" + "\n";
            }
            if (cbGrossTime.IsChecked == true && string.IsNullOrWhiteSpace(txtGrossTime.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtGrossTime);
                }
                ErrorMsg += "Please insert display name of gross time" + "\n";
            }
            if (cbTareWeight.IsChecked == true && string.IsNullOrWhiteSpace(txtTareWeight.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtTareWeight);
                }
                ErrorMsg += "Please insert display name of tare weight" + "\n";
            }
            if (cbTareTime.IsChecked == true && string.IsNullOrWhiteSpace(txtTareTime.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtTareTime);
                }
                ErrorMsg += "Please insert display name of tare time" + "\n";
            }
            if (cbNetWeight.IsChecked == true && string.IsNullOrWhiteSpace(txtNetWeight.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtNetWeight);
                }
                ErrorMsg += "Please insert display name of net weight" + "\n";
            }
            if (cbProduct.IsChecked == true && string.IsNullOrWhiteSpace(txtProduct.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtProduct);
                }
                ErrorMsg += "Please insert display name of Product" + "\n";
            }
            if (cbCharges.IsChecked == true && string.IsNullOrWhiteSpace(txtCharges.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtCharges);
                }
                ErrorMsg += "Please insert display name of charges" + "\n";
            }
            if (cbField1.IsChecked == true && string.IsNullOrWhiteSpace(txtField1.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField1);
                }
                ErrorMsg += "Please insert display name of " + lblField1.Content + "\n";
            }
            if (cbField2.IsChecked == true && string.IsNullOrWhiteSpace(txtField2.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField2);
                }
                ErrorMsg += "Please insert display name of " + lblField2.Content + "\n";
            }
            if (cbField3.IsChecked == true && string.IsNullOrWhiteSpace(txtField3.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField3);
                }
                ErrorMsg += "Please insert display name of " + lblField3.Content + "\n";
            }
            if (cbField4.IsChecked == true && string.IsNullOrWhiteSpace(txtField4.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField4);
                }
                ErrorMsg += "Please insert display name of " + lblField4.Content + "\n";
            }
            if (cbField5.IsChecked == true && string.IsNullOrWhiteSpace(txtField5.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField5);
                }
                ErrorMsg += "Please insert display name of " + lblField5.Content + "\n";
            }
            if (cbField6.IsChecked == true && string.IsNullOrWhiteSpace(txtField6.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField6);
                }
                ErrorMsg += "Please insert display name of " + lblField6.Content + "\n";
            }
            if (cbField7.IsChecked == true && string.IsNullOrWhiteSpace(txtField7.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField7);
                }
                ErrorMsg += "Please insert display name of " + lblField7.Content + "\n";
            }
            if (cbField8.IsChecked == true && string.IsNullOrWhiteSpace(txtField8.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField8);
                }
                ErrorMsg += "Please insert display name of " + lblField8.Content;
            }
            if (cbField9.IsChecked == true && string.IsNullOrWhiteSpace(txtField9.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField9);
                }
                ErrorMsg += "Please insert display name of " + lblField9.Content + "\n";
            }
            if (cbField10.IsChecked == true && string.IsNullOrWhiteSpace(txtField10.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField10);
                }
                ErrorMsg += "Please insert display name of " + lblField10.Content + "\n";
            }
            if (cbField11.IsChecked == true && string.IsNullOrWhiteSpace(txtField11.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField11);
                }
                ErrorMsg += "Please insert display name of " + lblField11.Content + "\n";
            }
            if (cbField12.IsChecked == true && string.IsNullOrWhiteSpace(txtField12.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField12);
                }
                ErrorMsg += "Please insert display name of " + lblField12.Content + "\n";
            }
            if (cbField13.IsChecked == true && string.IsNullOrWhiteSpace(txtField13.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField13);
                }
                ErrorMsg += "Please insert display name of " + lblField13.Content + "\n";
            }
            if (cbField14.IsChecked == true && string.IsNullOrWhiteSpace(txtField14.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField14);
                }
                ErrorMsg += "Please insert display name of " + lblField14.Content + "\n";
            }
            if (cbField15.IsChecked == true && string.IsNullOrWhiteSpace(txtField15.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField15);
                }
                ErrorMsg += "Please insert display name of " + lblField15.Content + "\n";
            }
            if (cbField16.IsChecked == true && string.IsNullOrWhiteSpace(txtField16.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField16);
                }
                ErrorMsg += "Please insert display name of " + lblField16.Content + "\n";
            }
            if (cbField17.IsChecked == true && string.IsNullOrWhiteSpace(txtField17.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField17);
                }
                ErrorMsg += "Please insert display name of " + lblField17.Content + "\n";
            }
            if (cbField18.IsChecked == true && string.IsNullOrWhiteSpace(txtField18.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField18);
                }
                ErrorMsg += "Please insert display name of " + lblField18.Content + "\n";
            }
            if (cbField19.IsChecked == true && string.IsNullOrWhiteSpace(txtField19.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField19);
                }
                ErrorMsg += "Please insert display name of " + lblField19.Content + "\n";
            }
            if (cbField20.IsChecked == true && string.IsNullOrWhiteSpace(txtField20.Text))
            {
                if (!Isfocus)
                {
                    Isfocus = true;
                    Keyboard.Focus(txtField20);
                }
                ErrorMsg += "Please insert display name of " + lblField20.Content + "\n";
            }
            if (ErrorMsg != "")
            {
                ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Following details are required", ErrorMsg);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
