using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using WeighingSolution.Helpers;
using WeighingSolution.DataLayer;
using System.Windows;
using System.ComponentModel;
using WeighingSolution.SharedComponents;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data;
using System.Threading;
using System.Windows.Shapes;
using System.Windows.Media;

namespace WeighingSolution.ViewModel
{
    public class DashboardViewModel : NotifyPropertyChanged, IDisposable 
    {
        public Page parentPage;
        public DataRepository dataRepository;
       
        private static string NowStamp() => $" [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]";

        private string _UHFDeta;
        public string UHFDeta
        {
            get { return _UHFDeta; }
            set { _UHFDeta = value; OnPropertyChanged(); }
        }
        private string _sensore1;
        public string sensore1
        {
            get { return _sensore1; }
            set { _sensore1 = value; OnPropertyChanged(); }
        }
        private string _bgsensore1;
        public string bgsensore1
        {
            get { return _bgsensore1; }
            set { _bgsensore1 = value; OnPropertyChanged(); }
        }
       
        
        private bool _IsBoomLeft;
        public bool IsBoomLeft
        {
            get { return _IsBoomLeft; }
            set { _IsBoomLeft = value; OnPropertyChanged(); }
        }

        private string _DisplayWeight;
        public string DisplayWeight
        {
            get { return _DisplayWeight; }
            set { _DisplayWeight = value; OnPropertyChanged(); }
        }

        private string _Displaycolor;
        public string Displaycolor
        {
            get { return _Displaycolor; }
            set { _Displaycolor = value; OnPropertyChanged(); }
        }
        private string _Display_Massage;
        public string Display_Massag
        {
            get { return _Display_Massage; }
            set { _Display_Massage = value; OnPropertyChanged(); }
        }
        private string _DisplayVehicle;
        public string DisplayVehicle
        {
            get { return _DisplayVehicle; }
            set { _DisplayVehicle = value; OnPropertyChanged(); }
        }
        private string _colorVehicle;
        public string ColorVehicle
        {
            get { return _colorVehicle; }
            set { _colorVehicle = value; OnPropertyChanged(); }
        }
        private string _Displayrfidleft;
        public string Displayrfidleft
        {
            get { return _Displayrfidleft; }
            set { _Displayrfidleft = value; OnPropertyChanged(); }
        }
        private string _Displayrfidright;
        public string Displayrfidright
        {
            get { return _Displayrfidright; }
            set { _Displayrfidright = value; OnPropertyChanged(); }
        }
        private bool _IsBoomRight;

        public bool IsBoomRight
        {
            get { return _IsBoomRight; }
            set { _IsBoomRight = value; OnPropertyChanged(); }
        }

        private bool _IsTRight;

        public bool IsTRight
        {
            get { return _IsTRight; }
            set { _IsTRight = value; OnPropertyChanged(); }
        }

        private bool _IsTLeft;

        public bool IsTLeft
        {
            get { return _IsTLeft; }
            set { _IsTLeft = value; OnPropertyChanged(); }
        }
        private bool _IsSensore1;
        public bool IsSensore1
        {
            get { return _IsSensore1; }
            set { _IsSensore1 = value; OnPropertyChanged(); }
        }
        private bool _IsSensore2;
        public bool IsSensore2
        {
            get { return _IsSensore2; }
            set { _IsSensore2 = value; OnPropertyChanged(); }
        }

        private bool _IsSensore3;
        public bool IsSensore3
        {
            get { return _IsSensore3; }
            set { _IsSensore3 = value; OnPropertyChanged(); }
        }


        private bool _IsUHFLeft;
        public bool IsUHFLeft
        {
            get { return _IsUHFLeft; }
            set { _IsUHFLeft = value; OnPropertyChanged(); }
        }


        private bool _IsUHFRight;
        public bool IsUHFRight
        {
            get { return _IsUHFRight; }
            set { _IsUHFRight = value; OnPropertyChanged(); }
        }

        #region Left cycle

        private Visibility _Left_TImg_Card_Tap;
        public Visibility Left_TImg_Card_Tap
        {
            get { return _Left_TImg_Card_Tap; }
            set { _Left_TImg_Card_Tap = value; OnPropertyChanged(); }
        }

        private Visibility _Left_TImg_S2_Cut;
        public Visibility Left_TImg_S2_Cut
        {
            get { return _Left_TImg_S2_Cut; }
            set { _Left_TImg_S2_Cut = value; OnPropertyChanged(); }
        }

        private Visibility _Left_TImg_WT_Start;
        public Visibility Left_TImg_WT_Start
        {
            get { return _Left_TImg_WT_Start; }
            set { _Left_TImg_WT_Start = value; OnPropertyChanged(); }
        }

        private Visibility _Left_TImg_S3_Cut;
        public Visibility Left_TImg_S3_Cut
        {
            get { return _Left_TImg_S3_Cut; }
            set { _Left_TImg_S3_Cut = value; OnPropertyChanged(); }
        }

        private Visibility _Left_TImg_S3_UnCut;
        public Visibility Left_TImg_S3_UnCut
        {
            get { return _Left_TImg_S3_UnCut; }
            set { _Left_TImg_S3_UnCut = value; OnPropertyChanged(); }
        }

        #endregion Left cycle

        #region Right cycle

        private Visibility _Right_TImg_Card_Tap;
        public Visibility Right_TImg_Card_Tap
        {
            get { return _Right_TImg_Card_Tap; }
            set { _Right_TImg_Card_Tap = value; OnPropertyChanged(); }
        }

        private Visibility _Right_TImg_S3_Cut;
        public Visibility Right_TImg_S3_Cut
        {
            get { return _Right_TImg_S3_Cut; }
            set { _Right_TImg_S3_Cut = value; OnPropertyChanged(); }
        }

        private Visibility _Right_TImg_WT_Start;
        public Visibility Right_TImg_WT_Start
        {
            get { return _Right_TImg_WT_Start; }
            set { _Right_TImg_WT_Start = value; OnPropertyChanged(); }
        }

        private Visibility _Right_TImg_S2_Cut;
        public Visibility Right_TImg_S2_Cut
        {
            get { return _Right_TImg_S2_Cut; }
            set { _Right_TImg_S2_Cut = value; OnPropertyChanged(); }
        }

        private Visibility _Right_TImg_S2_UnCut;
        public Visibility Right_TImg_S2_UnCut
        {
            get { return _Right_TImg_S2_UnCut; }
            set { _Right_TImg_S2_UnCut = value; OnPropertyChanged(); }
        }

        #endregion Right cycle


        public DashboardViewModel(Page page)
        {
            parentPage = page;
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));


            Left_TImg_Card_Tap = Visibility.Collapsed;
            Left_TImg_S2_Cut = Visibility.Collapsed;
            Left_TImg_WT_Start = Visibility.Collapsed;
            Left_TImg_S3_Cut = Visibility.Collapsed;
            Left_TImg_S3_UnCut = Visibility.Collapsed;

            Right_TImg_Card_Tap = Visibility.Collapsed;
            Right_TImg_S3_Cut = Visibility.Collapsed;
            Right_TImg_WT_Start = Visibility.Collapsed;
            Right_TImg_S2_Cut = Visibility.Collapsed;
            Right_TImg_S2_UnCut = Visibility.Collapsed;

            

        }

        public void SetHwComponent()
        {
            while (true)
            {
                Thread.Sleep(200);

                Displayrfidleft = "Left Side: " + Constant.DisplayRFID_Left;
                Displayrfidright = "Right Side: " + Constant.DisplayRFID_Right;
                Display_Massag = Constant.Display_Message;
                if (!string.IsNullOrEmpty(Constant.Display_Vehicle))
                {
                    DisplayVehicle = "Truck No: " + Constant.Display_Vehicle;
                    ColorVehicle = Constant.Color_Vehicle;
                }
                else
                {
                    DisplayVehicle = "Truck No: ";
                    ColorVehicle = "Black";
                }

                Displaycolor = Constant.IsWeighSaved? "Green": "Black";

                TruckImageSetup();

                IsSensore1 = Constant.IsSecoundSensorOn;
                IsSensore3 = Constant.IsTrirdSensorOn;

                IsTLeft = Constant.IsTL;
                IsTRight = Constant.IsTR;

                IsBoomLeft = Constant.IsBoomLeft;
                IsBoomRight = Constant.IsBoomRight;

                DisplayWeight = $"Weight : {Constant.IndicatorWeight.ToString()}";
            }
        }

        public void TruckImageSetup()
        {
            Left_TImg_Card_Tap = Visibility.Collapsed;
            Left_TImg_S2_Cut = Visibility.Collapsed;
            Left_TImg_WT_Start = Visibility.Collapsed;
            Left_TImg_S3_Cut = Visibility.Collapsed;
            Left_TImg_S3_UnCut = Visibility.Collapsed;

            Right_TImg_Card_Tap = Visibility.Collapsed;
            Right_TImg_S3_Cut = Visibility.Collapsed;
            Right_TImg_WT_Start = Visibility.Collapsed;
            Right_TImg_S2_Cut = Visibility.Collapsed;
            Right_TImg_S2_UnCut = Visibility.Collapsed;

            IsUHFLeft = false;

            #region Left to Right
            if (Constant.WBSIDE_DASHBORD == "Left")
            {
                IsUHFLeft = true;
                Left_TImg_Card_Tap = Visibility.Visible;
            }

            if(Constant.IsVehicleEnter == false && Constant.WBSIDE == "Left")
            {
                Left_TImg_Card_Tap = Visibility.Visible;
            }

            if (Constant.IsSecoundSensorOn && !string.IsNullOrEmpty(Constant.WBSIDE) && Constant.WBSIDE == "Left")
            {
                Constant.IsVehicleEnter = true;
                Left_TImg_Card_Tap = Visibility.Collapsed;
                Left_TImg_S2_Cut = Visibility.Visible;
            }            

            if (Constant.IsWeighDashStart && Constant.WBSIDE == "Left")
            {
                Left_TImg_WT_Start = Visibility.Visible;
            }

            if (Constant.IsWeighSaved && Constant.IsTrirdSensorOn && Constant.WBSIDE == "Left")
            {
                Constant.IsWeighDashStart = false;
                Left_TImg_WT_Start = Visibility.Collapsed;
                Left_TImg_S3_Cut = Visibility.Visible;
            }

            if (Constant.IsWeighSaved && Constant.IsUnmanCycle_end && Constant.WBSIDE == "Left")
            {
                Left_TImg_S3_UnCut = Visibility.Visible;
            }

            #endregion Left to Right

            #region Right to Left

            IsUHFRight = false;

            if (Constant.WBSIDE_DASHBORD == "Right")
            {
                IsUHFRight = true;
                Right_TImg_Card_Tap = Visibility.Visible;
            }

            if (Constant.IsVehicleEnter == false && Constant.WBSIDE == "Right")
            {
                Right_TImg_Card_Tap = Visibility.Visible;
            }

            if (Constant.IsTrirdSensorOn && !string.IsNullOrEmpty(Constant.WBSIDE) && Constant.WBSIDE == "Right")
            {
                Constant.IsVehicleEnter = true;
                Right_TImg_Card_Tap = Visibility.Collapsed;
                Right_TImg_S3_Cut = Visibility.Visible;
            }

            if (Constant.IsWeighDashStart && Constant.WBSIDE == "Right")
            {
                Right_TImg_WT_Start = Visibility.Visible;
            }

            if (Constant.IsWeighSaved && Constant.IsSecoundSensorOn && Constant.WBSIDE == "Right")
            {
                Constant.IsWeighDashStart = false;
                Right_TImg_WT_Start = Visibility.Collapsed;
                Right_TImg_S2_Cut = Visibility.Visible;
            }

            if (Constant.IsWeighSaved && Constant.IsUnmanCycle_end && Constant.WBSIDE == "Right")
            {
                Right_TImg_S2_UnCut = Visibility.Visible;
            }

            #endregion Right to Left

            //if(Constant.Final_end && string.IsNullOrEmpty(Constant.WBSIDE))
            //{
            //    Left_TImg_Card_Tap = Visibility.Collapsed;
            //    Left_TImg_S2_Cut = Visibility.Collapsed;
            //    Left_TImg_WT_Start = Visibility.Collapsed;
            //    Left_TImg_S3_Cut = Visibility.Collapsed;
            //    Left_TImg_S3_UnCut = Visibility.Collapsed;

            //    Right_TImg_Card_Tap = Visibility.Collapsed;
            //    Right_TImg_S3_Cut = Visibility.Collapsed;
            //    Right_TImg_WT_Start = Visibility.Collapsed;
            //    Right_TImg_S2_Cut = Visibility.Collapsed;
            //    Right_TImg_S2_UnCut = Visibility.Collapsed;
            //}

        }

        public void Dispose()
        {
            try
            {
               
                dataRepository.Context.Dispose();
            }
            catch { }
        }
    }
}