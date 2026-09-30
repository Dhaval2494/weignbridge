using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    class vmUserManagement : DataSetupEditViewModelBase<UserMaster>
    {
        public vmUserManagement(int UserID)
        {
            Id = UserID;
        }
        private List<Template> template;
        public List<Template> Templates
        {
            get { return template; }
            set { template = value; OnPropertyChanged(); }
        }
        public override async Task LoadAsync()
        {
            Templates = await dataRepository.Context.Templates.AsNoTracking().Where(x => x.Active == true).ToListAsync();

            var usermaster = Id > 0
              ? await dataRepository.Context.UserMasters.SingleAsync(x => x.UserID == Id)
              : CreateNewUser();

            if (usermaster.UserID > 0)
            {
                usermaster.Password = Common.DecryptData(usermaster.Password);
                usermaster.ConfirmPassword = usermaster.Password;
            }
            InitializeModel(usermaster);
        }
        private UserMaster CreateNewUser()
        {
            var usermaster = CreateNewModel();
            usermaster.Active = true;
            usermaster.TemplateID = Templates.FirstOrDefault(x => x.TemplateName.ToLower().Trim() == "admin").TemplateID;
            return usermaster;
        }
        protected async override void AfterSaveExecute()
        {
            Model.Salt = Common.createSalt(Model.UserName, Model.Password, Model);
            Model.Password = Common.EncryptData(Model.Password);
            await dataRepository.SaveAsync();
            ToastHelper.ShowSuccess("User information recorded successfully");
            RaiseDetailSavedEvent(Model.UserID, Model);
        }
    }
}