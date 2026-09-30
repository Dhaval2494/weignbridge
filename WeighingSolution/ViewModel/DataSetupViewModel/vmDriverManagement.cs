using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class vmDriverManagement : DataSetupEditViewModelBase<Driver>
    {
        public DelegateCommand<String> SelectFile { get; private set; }
        public vmDriverManagement(int DriverID)
        {
            Id = DriverID;
            SelectFile = new DelegateCommand<String>(OnSelectFile);
        }

        private void OnSelectFile(String ImageSource)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            bool? result = open.ShowDialog();

            if (result == true)
            {
                switch (ImageSource)
                {
                    case "Photo":
                        Model.Photo = open.FileName;
                        break;
                    case "IDProofScan":
                        Model.IDProofScan = open.FileName;
                        break;
                    default:
                        break;
                }
            }
        }

        private List<AccountBindingModel> accounts;
        public List<AccountBindingModel> Accounts
        {
            get { return accounts; }
            set { accounts = value; OnPropertyChanged(); }
        }

        public override async Task LoadAsync()
        {
            var driver = Id > 0
              ? await dataRepository.Context.Drivers.SingleAsync(x => x.DriverID == Id)
              : CreateNewDriver();

            //Accounts = await dataRepository.Context.Accounts.AsNoTracking().Where(x => x.Active == true).ToListAsync();
            Accounts = await dataRepository.Context.Accounts.AsNoTracking().Where(x => x.Active == true).Select(x => new AccountBindingModel { AccountID = x.AccountID, CompanyName = x.CompanyName }).OrderBy(p => p.CompanyName).ToListAsync();
            Accounts.Insert(0, new AccountBindingModel() { AccountID = 0, CompanyName = "-- Select Account --" });
            //Accounts.Insert(0, new Account() { AccountID = 0, CompanyName = "-- Select Account --" });

            InitializeModel(driver);
        }

        private Driver CreateNewDriver()
        {
            var driver = CreateNewModel();
            driver.Active = true;
            return driver;
        }

        private string SavePhoto(string FullFilePath, object RemovePhotoPath)
        {
            string FileExtn = System.IO.Path.GetExtension(FullFilePath);
            string FileName = Guid.NewGuid().ToString("N");
            string NewFilePath = Constant.CustomImagesPath + @"\" + FileName + FileExtn;
            File.Copy(FullFilePath, NewFilePath, true);

            if (RemovePhotoPath != null && !String.IsNullOrEmpty(RemovePhotoPath.ToString()))
            {
                string oldPath = Constant.CustomImagesPath + @"\" + RemovePhotoPath;
                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            return FileName + FileExtn;
        }

        protected override bool BeforeSaveExecute()
        {
            DbPropertyEntry prop = dataRepository.Context.Entry(Model).Property("Photo");
            if ((prop.EntityEntry.State == EntityState.Added && prop.CurrentValue != null && !String.IsNullOrEmpty(prop.CurrentValue.ToString()))
                || (prop.IsModified && !prop.CurrentValue.Equals(prop.OriginalValue)))
            {
                Model.Photo = SavePhoto(Model.Photo, prop.CurrentValue);
            }

            prop = dataRepository.Context.Entry(Model).Property("IDProofScan");
            if ((prop.EntityEntry.State == EntityState.Added && prop.CurrentValue != null && !String.IsNullOrEmpty(prop.CurrentValue.ToString()))
                    || (prop.IsModified && !prop.CurrentValue.Equals(prop.OriginalValue)))
            {
                Model.IDProofScan = SavePhoto(Model.IDProofScan, prop.CurrentValue);
            }

            return true;
        }
        protected override void AfterSaveExecute()
        {
            ToastHelper.ShowSuccess("Driver information recorded successfully");
            RaiseDetailSavedEvent(Model.DriverID, Model);
        }
    }
}