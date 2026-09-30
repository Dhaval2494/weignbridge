using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.CustomControls;
using WeighingSolution.DataLayer;

namespace WeighingSolution.View
{
    public partial class BarcodePage : Page
    {
        //public PluginHelper PluginHelper = new PluginHelper();
        private string BarcodeOrQr { get; set; }
        public BarcodePage()
        {
            InitializeComponent();
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
                {
                    BarcodeOrQr = ctx.ApplicationSettings.FirstOrDefault(x => x.SettingName == "BarcodeOrQR").SettingValue;
                    if (string.IsNullOrEmpty(BarcodeOrQr))
                    {
                        cmbBarcodeOrQR.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbBarcodeOrQR.SelectedValue = BarcodeOrQr;
                    }

                    List<CustomField> lstcstmfield = ctx.CustomFields.ToList();

                    foreach (var item in lstcstmfield)
                    {
                        string lblFieldName = $"lbl{item.FieldName}";
                        Label label = this.FindName(lblFieldName) as Label;
                        label.Content = lstcstmfield.FirstOrDefault(x => x.FieldName == item.FieldName).DisplayName;
                    }

                    List<BarcodeSetting> lstBarcode = ctx.BarcodeSettings.ToList();
                    if (lstBarcode != null && lstBarcode.Count > 0)
                    {
                        SetBarcodeSetting(lstBarcode, "Ticket No", cbTicketNo, txtTicketNo);
                        SetBarcodeSetting(lstBarcode, "Vehicle No", cbVehicleNo, txtVehicleNo);
                        SetBarcodeSetting(lstBarcode, "Account Name", cbAccountName, txtAccountName);
                        SetBarcodeSetting(lstBarcode, "Gross Weight", cbGrossWeight, txtGrossWeight);
                        SetBarcodeSetting(lstBarcode, "Gross Time", cbGrossTime, txtGrossTime);
                        SetBarcodeSetting(lstBarcode, "Tare Weight", cbTareWeight, txtTareWeight);
                        SetBarcodeSetting(lstBarcode, "Tare Time", cbTareTime, txtTareTime);
                        SetBarcodeSetting(lstBarcode, "Net Weight", cbNetWeight, txtNetWeight);
                        SetBarcodeSetting(lstBarcode, "Charges", cbCharges, txtCharges);
                        SetBarcodeSetting(lstBarcode, "Product", cbProduct, txtProduct);

                        foreach (var item in lstcstmfield)
                        {
                            string FieldName = item.FieldName;
                            string cbFieldName = $"cb{item.FieldName}";
                            string txtFieldName = $"txt{item.FieldName}";
                            CheckBox checkBox = this.FindName(cbFieldName) as CheckBox;
                            FilteredTextbox filteredTextbox = this.FindName(txtFieldName) as FilteredTextbox;

                            SetBarcodeSetting(lstBarcode, FieldName, checkBox, filteredTextbox);
                        }
                    }
                }
            }
            catch { }
        }

        public void SetBarcodeSetting(List<BarcodeSetting> lstBarcode, string BarcodeFieldName, CheckBox checkBox, FilteredTextbox filteredTextbox)
        {
            BarcodeSetting objField = lstBarcode.FirstOrDefault(x => x.BarcodeFieldName == BarcodeFieldName);
            if (objField != null)
            {
                checkBox.IsChecked = objField.IsSelected;
                filteredTextbox.Text = objField.BarcodeDisplayName.Trim();
            }
        }

        private void btnSaveBarcode_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidate()) return;

            using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                List<BarcodeSetting> lstBarcode = ctx.BarcodeSettings.ToList();
                var settings = ctx.BarcodeSettings.ToList(); // load once

                // Fixed/known fields (with label + textbox + checkbox triplets)
                SaveField(ctx, settings, "Ticket No", lblTicketNo, txtTicketNo, cbTicketNo);
                SaveField(ctx, settings, "Vehicle No", lblVehicleNo, txtVehicleNo, cbVehicleNo);
                SaveField(ctx, settings, "Account Name", lblAccountName, txtAccountName, cbAccountName);
                SaveField(ctx, settings, "Gross Weight", lblGrossWeight, txtGrossWeight, cbGrossWeight);
                SaveField(ctx, settings, "Gross Time", lblGrossTime, txtGrossTime, cbGrossTime);
                SaveField(ctx, settings, "Tare Weight", lblTareWeight, txtTareWeight, cbTareWeight);
                SaveField(ctx, settings, "Tare Time", lblTareTime, txtTareTime, cbTareTime);
                SaveField(ctx, settings, "Net Weight", lblNetWeight, txtNetWeight, cbNetWeight);
                SaveField(ctx, settings, "Charges", lblCharges, txtCharges, cbCharges);
                SaveField(ctx, settings, "Product", lblProduct, txtProduct, cbProduct);

                // Custom / extra fields (Field1 → Field20)
                for (int i = 1; i <= 20; i++)
                {
                    var lbl = (Label)FindName($"lblField{i}");
                    var txt = (TextBox)FindName($"txtField{i}");
                    var cb = (CheckBox)FindName($"cbField{i}");
                    if (lbl == null || txt == null || cb == null) continue;
                    SaveField(ctx, settings, $"Field{i}", lbl, txt, cb);
                }

                // Optional: save Barcode vs QR mode
                if (BarcodeOrQr != null)
                {
                    var qrSetting = ctx.ApplicationSettings.FirstOrDefault(x => x.SettingName == "BarcodeOrQR");

                    if (qrSetting != null)
                        qrSetting.SettingValue = BarcodeOrQr;
                }
                ctx.SaveChanges();
                WeighingSolution.Helpers.ToastHelper.ShowSuccess("Barcode Template saved successfully");
                NavigationService?.Navigate(new Uri("/View/SettingView/Setting_Page.xaml", UriKind.Relative));
            }
        }

        // Helper method — handles both create & update
        private void SaveField(weighbridgeEntities ctx, List<BarcodeSetting> existing, string fieldKey, Label label, TextBox displayNameText, CheckBox checkBox)
        {
            var entity = existing.FirstOrDefault(x => x.BarcodeFieldName == fieldKey);
            if (entity == null)
            {
                entity = new BarcodeSetting
                {
                    BarcodeFieldName = fieldKey,   // ← important: use the internal key
                };
                ctx.BarcodeSettings.Add(entity);
            }
            entity.BarcodeDisplayName = displayNameText?.Text?.Trim() ?? "";
            entity.IsSelected = checkBox?.IsChecked == true;
        }
        private bool IsValidate()
        {
            var errors = new List<string>();
            var firstInvalidControl = default(UIElement);

            void CheckField(CheckBox cb, TextBox txt, string fieldLabel)
            {
                if (cb.IsChecked != true) return;

                var value = txt.Text?.Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                    errors.Add($"Please insert display name of {fieldLabel}");
                    if (firstInvalidControl == null) firstInvalidControl = txt;
                }
            }

            CheckField(cbTicketNo, txtTicketNo, "ticket no");
            CheckField(cbVehicleNo, txtVehicleNo, "vehicle no");
            CheckField(cbAccountName, txtAccountName, "account name");
            CheckField(cbGrossWeight, txtGrossWeight, "gross weight");
            CheckField(cbGrossTime, txtGrossTime, "gross time");
            CheckField(cbTareWeight, txtTareWeight, "tare weight");
            CheckField(cbTareTime, txtTareTime, "tare time");
            CheckField(cbNetWeight, txtNetWeight, "net weight");
            CheckField(cbCharges, txtCharges, "charges");
            CheckField(cbProduct, txtProduct, "product");

            // Custom fields
            for (int i = 1; i <= 20; i++)
            {
                var cb = (CheckBox)FindName($"cbField{i}");
                var txt = (TextBox)FindName($"txtField{i}");
                var lbl = (Label)FindName($"lblField{i}");

                if (cb == null || txt == null || lbl == null) continue;

                string labelText = lbl.Content?.ToString()?.Trim() ?? $"Field {i}";
                CheckField(cb, txt, labelText.ToLowerInvariant());
            }

            if (errors.Count == 0) return true;

            if (firstInvalidControl != null)
                Keyboard.Focus(firstInvalidControl);

            ((MetroWindow)Application.Current.MainWindow)
                .ShowMessageAsync("Following details are required", string.Join("\n", errors));

            return false;
        }

        private void BarcodePageCancel_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void btnBarcodeSetting_Click(object sender, RoutedEventArgs e)
        {
            BarcodeImagePathSetting childWindow = new BarcodeImagePathSetting();
            ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, childWindow);
        }

        private void cmbBarcodeOrQR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                if (cmbBarcodeOrQR.SelectedIndex == 0)
                {
                    BarcodeOrQr = string.Empty;
                }
                else
                {
                    if (cmbBarcodeOrQR.SelectedValue != null)
                        BarcodeOrQr = cmbBarcodeOrQR.SelectedValue.ToString();
                }
            }
        }
    }
}
