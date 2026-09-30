using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class vmVehicleManagement : DataSetupEditViewModelBase<Vehicle>
    {
        private Nullable<decimal> _OldTareWeight;
        public Nullable<decimal> OldTareWeight
        {
            get { return _OldTareWeight; }
            protected set { _OldTareWeight = value; }
        }
        public vmVehicleManagement(int VehicleID)
        {
            Id = VehicleID;
        }
        private List<AccountBindingModel> accounts;
        public List<AccountBindingModel> Accounts
        {
            get { return accounts; }
            set { accounts = value; OnPropertyChanged(); }
        }
        public override async Task LoadAsync()
        {
            var vehicle = Id > 0
              ? await dataRepository.Context.Vehicles.SingleAsync(x => x.VehicleID == Id)
              : CreateNewVehicle();

            OldTareWeight = vehicle.TareWeight;

            //Accounts = await dataRepository.Context.Accounts.AsNoTracking().Where(x => x.Active == true).OrderBy(p=>p.CompanyName).ToListAsync();
            Accounts = await dataRepository.Context.Accounts.AsNoTracking().Where(x => x.IsAccount == true && x.Active == true).Select(x => new AccountBindingModel { AccountID = x.AccountID, CompanyName = x.CompanyName }).OrderBy(p => p.CompanyName).ToListAsync();
            Accounts.Insert(0, new AccountBindingModel() { AccountID = 0, CompanyName = "-- Select Account --" });
            //Accounts.Insert(0, new Account() { AccountID = 0, CompanyName = "-- Select Account --" });
            InitializeModel(vehicle);
        }
        private Vehicle CreateNewVehicle()
        {
            var vehicle = CreateNewModel();
            vehicle.IsActive = true;
            return vehicle;
        }

        protected override bool BeforeSaveExecute()
        {
            if (Model.TareWeight.HasValue && OldTareWeight != Model.TareWeight)
                Model.TareWeightTime = DateTime.Now;

            return true;
        }

        protected override void AfterSaveExecute()
        {
            ToastHelper.ShowSuccess("Vehicle information recorded successfully");
            RaiseDetailSavedEvent(Model.VehicleID, Model);
        }
    }
}