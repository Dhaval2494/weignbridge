using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using System.Data.Entity;
using Prism.Commands;
using MahApps.Metro.Controls.Dialogs;
using WeighingSolution.Helpers;
using System.Windows;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class vmUHFManagement : DataSetupEditViewModelBase<UhfMaster>
    {
        public vmUHFManagement(int UhfID)
        {
            Id = UhfID;
        }

        private List<WeightBridge> weightBridges;
        public List<WeightBridge> WeightBridges
        {
            get { return weightBridges; }
            set { weightBridges = value; OnPropertyChanged(); }
        }

        public override async Task LoadAsync()
        {
            var uhfMaster = Id > 0
              ? await dataRepository.Context.UhfMasters.SingleAsync(x => x.ID == Id)
              : CreateNewUhfMaster();

            InitializeModel(uhfMaster);

            WeightBridges = dataRepository.Context.WeightBridges.AsNoTracking().Where(x => x.IsActive == true).OrderBy(p => p.ScaleName).ToList();
            WeightBridges.Insert(0, new WeightBridge() { WeightBridgeID = 0, ScaleName = "-- Select Scale --" });
        }
        private UhfMaster CreateNewUhfMaster()
        {
            var uhfMaster = CreateNewModel();
            uhfMaster.IsActive = true;
            return uhfMaster;
        }

        protected override bool BeforeSaveExecute()
        {
            System.Net.IPAddress ip;            
            bool ValidateIP = System.Net.IPAddress.TryParse(Model.IpAddress, out ip);
            if (!ValidateIP)
            {
                ToastHelper.ShowError("This is not a valid ip address");
                return false;
            }
            return true;
        }

        protected override void AfterSaveExecute()
        {
            ToastHelper.ShowSuccess("Uhf information recorded successfully");
            RaiseDetailSavedEvent(Model.ID, Model);
        }
    }
}