using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WeighingSolution.DataLayer
{
    public partial class TransactionData : NotifyDataErrorInfoBase
    {
        public string TransactionTitle
        {
            get
            {
                if (Vehicle != null && !String.IsNullOrEmpty(Vehicle.VehicleNumber))
                    return "Ticket No: " + TicketID.ToString() + " Vehical No: " + Vehicle.VehicleNumber;
                else
                    return "Ticket No: " + TicketID.ToString();
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

                        List<string> List = this.TransactionDetails.Where(p => p.ProductID != 0 && p.Product != null).Select(p => p.Product.ProductName).Distinct().ToList();

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

        public string CreationTimestringForListPage
        {
            get { return Constant.FormateDateTimeOutString(CreationTime); }
        }

        public string GateNameForListPage
        {
            get
            {
                return TransactionDetails?.LastOrDefault(x => x.ReceiptTicketID == ReceiptTicketID).Gate != null
                    ? TransactionDetails.LastOrDefault(x => x.ReceiptTicketID == ReceiptTicketID)?.Gate?.GateName
                    : null;
            }
        }

        public string ScaleNameForListPage
        {
            get
            {
                return TransactionDetails.LastOrDefault(x => x.ReceiptTicketID == ReceiptTicketID).WeightBridge != null
                    ? TransactionDetails.LastOrDefault(x => x.ReceiptTicketID == ReceiptTicketID).WeightBridge.ScaleName
                    : null;
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
                    TransactionDetail objLastPass = this.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault();
                    if (objLastPass != null)
                    {
                        if (objLastPass.TareWeight != null && objLastPass.CaptureWeight == objLastPass.TareWeight.Value)
                            return "Tare Weight: " + objLastPass.CaptureWeight.GetValueOrDefault(0).ToString(Constant.DecimalFormat) + objLastPass.WeightUnit;
                        else
                            return "Gross Weight: " + objLastPass.CaptureWeight.GetValueOrDefault(0).ToString(Constant.DecimalFormat) + objLastPass.WeightUnit;
                    }
                    else
                        return "";
                    //}
                    //else if (this.TransactionDetails.Count == 1)
                    //{
                    //    TransactionDetail objLastPass = this.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault();
                    //    return "" + this.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault().WeightUnit;
                    //}
                    //else
                    //{
                    //    return "";
                    //}
                }
                catch
                {
                    return "";
                }
            }
        }
        public TransactionDetail CompeletedTransaction
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
                        return this.TransactionDetails.Min(x => x.TareWeight).Value.ToString(Constant.DecimalFormat) + " " + this.TransactionDetails.FirstOrDefault(y => y.TareWeight == this.TransactionDetails.Min(x => x.TareWeight).Value).WeightUnit;
                    else
                        return "";
                }
                catch
                {
                    return "";
                }
            }
        }
        public string CompeletedTareWeightWithTime
        {
            get
            {
                try
                {
                    if (this.Status == "Complete")
                        return this.TransactionDetails.Min(x => x.TareWeight).Value.ToString(Constant.DecimalFormat) + " " + this.TransactionDetails.FirstOrDefault(y => y.TareWeight == this.TransactionDetails.Min(x => x.TareWeight).Value).WeightUnit + " (" + ((DateTime)this.TransactionDetails.FirstOrDefault(y => y.TareWeight == this.TransactionDetails.Min(x => x.TareWeight).Value).CaptureTime).ToString(Constant.DateTimeFormat) + ")";
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
                        return this.TransactionDetails.Max(x => x.GrossWeight).Value.ToString(Constant.DecimalFormat) + " " + this.TransactionDetails.FirstOrDefault(y => y.GrossWeight == this.TransactionDetails.Max(x => x.GrossWeight).Value).WeightUnit;
                    else
                        return "";
                }
                catch
                {
                    return "";
                }
            }

        }
        public string CompeletedGrossWeightWithTime
        {
            get
            {
                try
                {
                    if (this.Status == "Complete")
                        return this.TransactionDetails.Max(x => x.GrossWeight).Value.ToString(Constant.DecimalFormat) + " " + this.TransactionDetails.FirstOrDefault(y => y.GrossWeight == this.TransactionDetails.Max(x => x.GrossWeight).Value).WeightUnit + " (" + ((DateTime)this.TransactionDetails.FirstOrDefault(y => y.GrossWeight == this.TransactionDetails.Max(x => x.GrossWeight).Value).CaptureTime).ToString(Constant.DateTimeFormat) + ")";
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
                        return this.TransactionDetails.Sum(x => x.NetWeight).Value.ToString(Constant.DecimalFormat) + " " + this.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault().WeightUnit;
                    else
                        return "";
                }
                catch
                {
                    return "";
                }
            }

        }
        public ObservableCollection<TransactionDetail> ValidTransactionDetails
        {
            get
            {
                if (this.TransactionDetails == null)
                    return null;
                else if (this.TransactionDetails.Count > 1)
                {
                    if (this.TransactionDetails.Any(x => x.GrossWeight != null && x.GrossWeight != 0 && x.TareWeight != null && x.TareWeight != 0))
                        return new ObservableCollection<TransactionDetail>(this.TransactionDetails.Where(x => x.GrossWeight != null && x.GrossWeight != 0 && x.TareWeight != null && x.TareWeight != 0).ToList());
                    else
                        return new ObservableCollection<TransactionDetail>(this.TransactionDetails.Where(x => (x.GrossWeight != null && x.GrossWeight != 0) || (x.TareWeight != null && x.TareWeight != 0)).ToList());
                }
                else if (this.TransactionDetails.Count == 1)
                {
                    if (this.TransactionDetails.Any(x => (x.GrossWeight != null && x.GrossWeight != 0) || (x.TareWeight != null && x.TareWeight != 0)))
                        return new ObservableCollection<TransactionDetail>(this.TransactionDetails.ToList());
                    else
                        return null;
                }
                else
                    return new ObservableCollection<TransactionDetail>(this.TransactionDetails.ToList());
            }
        }
        public TransactionDetail SummaryTransaction
        {
            get { return ValidTransactionDetails != null ? ValidTransactionDetails.LastOrDefault() : null; }
        }
        public decimal? NetWeight
        {
            get { return ValidTransactionDetails != null ? ValidTransactionDetails.Where(t => t.NetWeight.HasValue).Sum(x => x.NetWeight) : null; }
        }
        #region Master Fields Properties
        private MasterField _accountMaster;
        public MasterField AccountMaster
        {
            get { return _accountMaster; }
            set { _accountMaster = value; OnPropertyChanged(); }
        }

        private MasterField _transporterMaster;
        public MasterField TransporterMaster
        {
            get { return _transporterMaster; }
            set { _transporterMaster = value; OnPropertyChanged(); }
        }

        private MasterField _driverMaster;
        public MasterField DriverMaster
        {
            get { return _driverMaster; }
            set { _driverMaster = value; OnPropertyChanged(); }
        }

        private MasterField _productMaster;
        public MasterField ProductMaster
        {
            get { return _productMaster; }
            set { _productMaster = value; OnPropertyChanged(); }
        }

        //private MasterField _rfidtokenMaster;
        //public MasterField RFIDTokenMaster
        //{
        //    get { return _rfidtokenMaster; }
        //    set { _rfidtokenMaster = value; OnPropertyChanged(); }
        //}

        private MasterField _gateMaster;
        public MasterField GateMaster
        {
            get { return _gateMaster; }
            set { _gateMaster = value; OnPropertyChanged(); }
        }

        private MasterField _chargesMaster;
        public MasterField ChargesMaster
        {
            get { return _chargesMaster; }
            set { _chargesMaster = value; OnPropertyChanged(); }
        }
        #endregion

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                //case "RFTokenID":
                //    if (RFIDTokenMaster != null && RFIDTokenMaster.IsRequired)
                //    {
                //        if (RFTokenID == null)
                //            yield return "RF Token ID is required.";
                //    }
                //    break;
                case "AccountID":
                    if (AccountMaster != null && AccountMaster.IsRequired)
                    {
                        if (AccountID == null)
                            yield return "Account is required.";
                    }
                    break;
                case "TransporterID":
                    if (TransporterMaster != null && TransporterMaster.IsRequired)
                    {
                        if (TransporterID == null)
                            yield return "Transporter is required.";
                    }
                    break;
                case "DriverID":
                    if (DriverMaster != null && DriverMaster.IsRequired)
                    {
                        if (DriverID == null)
                            yield return "Driver is required.";
                    }
                    break;
                case "Charges":
                    if (ChargesMaster != null && ChargesMaster.IsRequired)
                    {
                        if (Charges == null)
                            yield return "Charge is required.";
                    }
                    break;
            }
        }
    }
}
