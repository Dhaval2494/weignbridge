using System.Data.Entity;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    class vmAccountManagement : DataSetupEditViewModelBase<Account>
    {
        public vmAccountManagement(int AccountID)
        {
            Id = AccountID;
        }

        public override async Task LoadAsync()
        {
            var account = Id > 0
              ? await dataRepository.Context.Accounts.SingleAsync(x => x.AccountID == Id)
              : CreateNewAccount();

            InitializeModel(account);
        }
        protected override bool BeforeSaveExecute()
        {
            if ((Model.IsAccount == false || Model.IsAccount == null) && (Model.IsTransporter == false || Model.IsTransporter == null))
            {
                ToastHelper.ShowWarning("Please select role account or transporter");
                return false;
            }
            else
                return true;
        }
        private Account CreateNewAccount()
        {
            var account = CreateNewModel();
            account.Active = true;
            account.IsAccount = false;
            account.IsTransporter = false;
            return account;
        }
        protected override void AfterSaveExecute()
        {
            ToastHelper.ShowSuccess("Account information recorded successfully");
            RaiseDetailSavedEvent(Model.AccountID, Model);
        }
    }
}