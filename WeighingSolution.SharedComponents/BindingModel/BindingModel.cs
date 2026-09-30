using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace WeighingSolution.SharedComponents
{
    public class BindingModel
    {
    }
    public class CustomFieldModel
    {
        public int CustomFieldID { get; set; }
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
    }
    public class TokenBindingModel
    {
        public int TokenID { get; set; }
        public string TokenName { get; set; }
        public string TokenValue { get; set; }
        public string TokenType { get; set; }
        public string TokenStatus { get; set; }
        public Nullable<System.DateTime> TokenExpireDate { get; set; }
        public string PreferredWeighmentType { get; set; }
    }
    public class Receipt
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool IsMulti { get; set; }
        public string Image { get; set; }
    }
    public class UHFBindingListModel
    {
        public int ID { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool IsActive { get; set; }
        public int WeightbridgeID { get; set; }
        public string ScaleName { get; set; }
        public string WeightbridgeSide { get; set; }       
        public string UHFType { get; set; }
    }
    public class VehicleBindingModel
    {
        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; }
        public Nullable<decimal> TareWeight { get; set; }
    }
    public class VehicleBindingListModel
    {

        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleType { get; set; }
        public Nullable<decimal> TareWeight { get; set; }
        public string Notes { get; set; }
        public Nullable<int> AccountID { get; set; }
        public bool IsActive { get; set; }
        public string CompanyName { get; set; }
    }

    public class AccountBindingModel
    {
        public int AccountID { get; set; }
        public string CompanyName { get; set; }
    }
    public class AccountBindingListModel
    {
        public int AccountID { get; set; }
        public string AccountCode { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public Nullable<bool> IsAccount { get; set; }
        public Nullable<bool> IsTransporter { get; set; }
        public string Notes { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public bool Active { get; set; }
    }

    public class DriverBindingModel
    {
        public int DriverID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DriverName
        {
            get
            {
                return FirstName + (String.IsNullOrEmpty(LastName) ? "" : " " + LastName);
            }
        }
    }

    public class ProductBindingModel
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public bool IsActive { get; set; }
    }

    public class WeightBridgeConfigBindingModel
    {
        public int WeightBridgeID { get; set; }
        public string ScaleName { get; set; }
        public string SerialNo { get; set; }
    }

    public class TransactionDataBindingModel : NotifyPropertyChanged
    {
        public int TicketID { get; set; }
        public Nullable<int> DriverID { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public string Vehicle_Number { get; set; }
        public string TokenName { get; set; }
        public string Status { get; set; }
        public string TransactionMode { get; set; }
        public string CustomValue1 { get; set; }
        public string CustomValue2 { get; set; }
        public string CustomValue3 { get; set; }
        public string CustomValue4 { get; set; }
        public string CustomValue5 { get; set; }
        public string CustomValue6 { get; set; }
        public string CustomValue7 { get; set; }
        public string CustomValue8 { get; set; }
        public string CustomValue9 { get; set; }
        public string CustomValue10 { get; set; }
        public string CustomValue11 { get; set; }
        public string CustomValue12 { get; set; }
        public string CustomValue13 { get; set; }
        public string CustomValue14 { get; set; }
        public string CustomValue15 { get; set; }
        public string CustomValue16 { get; set; }
        public string CustomValue17 { get; set; }
        public string CustomValue18 { get; set; }
        public string CustomValue19 { get; set; }
        public string CustomValue20 { get; set; }
        public Nullable<int> AccountID { get; set; }
        public string Account_Name { get; set; }
        public string Driver_Name { get; set; }
        public Nullable<int> TransporterID { get; set; }
        public string Transporter_Name { get; set; }
        public System.Guid ReceiptTicketID { get; set; }
        public Nullable<decimal> Charges { get; set; }
        public string TransactionType { get; set; }
        public System.DateTime CreationTime { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        //public VehicleBindingModel Vehicle { get; set; }
        public DriverBindingModel Driver { get; set; }
        public ObservableCollection<TransactionDetailBindingModel> TransactionDetails { get; set; }

        public ObservableCollection<CustomFieldViewModel> _customFieldViewModels;
        public ObservableCollection<CustomFieldViewModel> CustomFieldViewModels
        {
            get => _customFieldViewModels;
            set
            {
                _customFieldViewModels = value;
                OnPropertyChanged();   // notify UI
            }
        }
        public string TransactionTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(Vehicle_Number))
                    return "Ticket No: " + TicketID.ToString() + " Vehical No: " + Vehicle_Number;
                else
                    return "Ticket No: " + TicketID.ToString();
            }
        }
        public string DateTime_Format { get; set; }
        public string CreationTimestringForListPage
        {
            get
            {
                if (CreationTime != null)
                    try
                    {
                        return CreationTime.ToString(DateTime_Format, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return CreationTime.ToString("dd-MM-yyyy HH:mm:");
                    }
                else
                    return "";
            }
        }
        public string TransactionProducts
        {
            get
            {
                try
                {
                    if (this.TransactionDetails != null && this.TransactionDetails.Count() > 0)
                    {
                        string products = string.Empty;

                        List<string> List = this.TransactionDetails.Select(p => p.ProductName).Distinct().ToList();

                        for (int i = 0; i < List.Count; i++)
                        {
                            string item = List[i];
                            products += item + ",";
                        }

                        if (!string.IsNullOrEmpty(products))
                            products = products.TrimEnd(',');
                        return products;
                    }
                    else
                        return "";
                }
                catch
                {
                    return "";
                }

            }
        }
        public string LastWeighment
        {
            get
            {
                try
                {
                    //if (this.TransactionDetails.Count > 1)
                    //{
                    TransactionDetailBindingModel objLastPass = this.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault();
                    if (objLastPass != null)
                    {
                        if (objLastPass.TareWeight != null && objLastPass.CaptureWeight == objLastPass.TareWeight.Value)
                            return "Tare Weight: " + objLastPass.CaptureWeight.GetValueOrDefault(0).ToString(DataLayer.Constant.DecimalFormat) + objLastPass.WeightUnit;
                        else return "Gross Weight: " + objLastPass.CaptureWeight.GetValueOrDefault(0).ToString(DataLayer.Constant.DecimalFormat) + objLastPass.WeightUnit;
                    }
                    else
                        return "";
                }
                catch
                {
                    return "";
                }
            }
        }
        public TransactionDetailBindingModel CompeletedTransaction
        {
            get { return this.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault(); }
        }
        public string CompeletedTareWeight
        {
            get
            {
                try
                {
                    if (this.Status == "Complete")
                        return this.TransactionDetails.Min(x => x.TareWeight).Value.ToString(DataLayer.Constant.DecimalFormat) + " " + this.TransactionDetails.FirstOrDefault(y => y.TareWeight == this.TransactionDetails.Min(x => x.TareWeight).Value).WeightUnit;
                    else
                        return "";
                }
                catch
                {
                    return "";
                }
            }
        }
        public string CompeletedGrossWeight
        {
            get
            {
                try
                {
                    if (this.Status == "Complete")
                        return this.TransactionDetails.Max(x => x.GrossWeight).Value.ToString(DataLayer.Constant.DecimalFormat) + " " + this.TransactionDetails.FirstOrDefault(y => y.GrossWeight == this.TransactionDetails.Max(x => x.GrossWeight).Value).WeightUnit;
                    else
                        return "";
                }
                catch
                {
                    return "";
                }
            }

        }
        public string CompeletedNetWeight
        {
            get
            {
                try
                {
                    if (this.Status == "Complete")
                        return this.TransactionDetails.Sum(x => x.NetWeight).Value.ToString(DataLayer.Constant.DecimalFormat) + " " + this.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault().WeightUnit;
                    else
                        return "";
                }
                catch
                {
                    return "";
                }
            }

        }
        public ObservableCollection<TransactionDetailBindingModel> ValidTransactionDetails
        {
            get
            {
                if (this.TransactionDetails == null)
                    return null;
                else if (this.TransactionDetails.Count > 1)
                {
                    if (this.TransactionDetails.Any(x => x.GrossWeight != null && x.GrossWeight != 0 && x.TareWeight != null && x.TareWeight != 0))
                        return new ObservableCollection<TransactionDetailBindingModel>(this.TransactionDetails.Where(x => x.GrossWeight != null && x.GrossWeight != 0 && x.TareWeight != null && x.TareWeight != 0).ToList());
                    else
                        return new ObservableCollection<TransactionDetailBindingModel>(this.TransactionDetails.Where(x => (x.GrossWeight != null && x.GrossWeight != 0) || (x.TareWeight != null && x.TareWeight != 0)).ToList());
                }
                else if (this.TransactionDetails.Count == 1)
                {
                    if (this.TransactionDetails.Any(x => (x.GrossWeight != null && x.GrossWeight != 0) || (x.TareWeight != null && x.TareWeight != 0)))
                        return new ObservableCollection<TransactionDetailBindingModel>(this.TransactionDetails.ToList());
                    else
                        return null;
                }
                else
                    return new ObservableCollection<TransactionDetailBindingModel>(this.TransactionDetails.ToList());
            }
        }
        public TransactionDetailBindingModel SummaryTransaction
        {
            get { return ValidTransactionDetails != null ? ValidTransactionDetails.LastOrDefault() : null; }
        }
        public decimal? NetWeight
        {
            get { return ValidTransactionDetails != null ? ValidTransactionDetails.Where(t => t.NetWeight.HasValue).Sum(x => x.NetWeight) : null; }
        }

        public string WeightBridgeName
        {
            get
            {
                return TransactionDetails?.FirstOrDefault()?.WeightBridgeName ?? string.Empty;
            }
        }

        public string GateNameForListPage
        {
            get
            {
                try
                {
                    if (this.TransactionDetails != null && this.TransactionDetails.Count > 0)
                    {
                        // Get distinct non-null Gate names
                        List<string> gates = this.TransactionDetails
                            .Where(td => !string.IsNullOrEmpty(td.GateName))
                            .Select(td => td.GateName)
                            .Distinct()
                            .ToList();

                        // Combine them as a comma-separated string
                        string gateNames = string.Join(", ", gates);

                        return gateNames;
                    }
                    else
                        return "";
                }
                catch
                {
                    return "";
                }
            }
        }



    }

    public partial class TransactionDetailBindingModel
    {
        public int TransactionDetailID { get; set; }
        public int WeightBridgeID { get; set; }
        public string WeightBridgeName { get; set; }
        public byte SequenceNo { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<decimal> GrossWeight { get; set; }
        public Nullable<decimal> TareWeight { get; set; }
        public Nullable<System.DateTime> GrossTime { get; set; }
        public Nullable<System.DateTime> TareTime { get; set; }
        public string WeighmentType { get; set; }
        public Nullable<int> GateID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<decimal> CaptureWeight { get; set; }
        public Nullable<System.DateTime> CaptureTime { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public string WeightUnit { get; set; }
        public bool IsTareManual { get; set; }
        public bool IsGrossManual { get; set; }
        public bool IsCapturedManual { get; set; }
        public System.Guid ReceiptTicketID { get; set; }

        public string ProductName { get; set; }
        public string GateName { get; set; }
        public string Wbusername { get; set; }
        //public ProductBindingModel Product { get; set; }

        //public GateBindingModel Gate { get; set; }

    }

    public class GateBindingModel
    {
        public int GateID { get; set; }
        public string GateType { get; set; }
        public string GateName { get; set; }
    }
}
