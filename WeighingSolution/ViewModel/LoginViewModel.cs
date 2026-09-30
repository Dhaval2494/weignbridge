using Prism.Commands;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    public class LoginViewModel : NotifyDataErrorInfoBase, IDisposable
    {
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        UserMaster objUserMaster;
        private Window parentPage;
        public DelegateCommand LoginCommand { get; private set; }
        public LoginViewModel(Window page)
        {
            parentPage = page;
            LoginCommand = new DelegateCommand(OnLoginExecute, OnLoginCanExecute);

            string queryu = $"exec sp_updatestats";
            bool flagu = DatabaseFunctions.ExecuteQuery(queryu);
            string query = $"dbcc freeproccache";
            bool flag = DatabaseFunctions.ExecuteQuery(query);
        }

        private bool OnLoginCanExecute()
        {
            if (HasChanges && !HasErrors)
                return true;
            else
                return false;
        }

        private void OnLoginExecute()
        {
            ValidatePropertyInternal("LoginUserName", _LoginUserName);
            ValidatePropertyInternal("LoginPassWord", _LoginPassWord);
            string ErrorMessage = "";
            bool Error = false;
            if (!HasErrors)
            {
                string query = "SELECT GETDATE()";
                DataTable localcheckdataTable = DatabaseFunctions.SQLExecuteReaderQuery(query);

                if (localcheckdataTable != null && localcheckdataTable.Rows.Count > 0)
                {
                    // Get server time from DataTable
                    DateTime serverTime = Convert.ToDateTime(localcheckdataTable.Rows[0][0]);

                    // Get local (client) system time
                    DateTime clientTime = DateTime.Now;

                    // Calculate time difference
                    TimeSpan diff = clientTime - serverTime;

                    // Check if the difference exceeds 2 minutes
                    if (Math.Abs(diff.TotalMinutes) > 2)
                    {
                        MessageBox.Show("Please Check Your System Time with server Time");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Please Check Your System Time with server Time");
                    return;
                }

                Constant.IsAdmin = false;
                Constant.IsSupportUser = false;

                if (LoginUserName != "" && objUserMaster == null)
                {
                    ErrorMessage = "User is not exist";
                }
                if (objUserMaster != null)
                {
                    if (!Common.IsSaltMatch(LoginUserName, LoginPassWord, objUserMaster))
                    {
                        ErrorMessage = "login information is incorrect";
                    }
                    else if (objUserMaster.Active == false)
                    {
                        ErrorMessage = "User is not active";
                    }
                    else if (!string.IsNullOrEmpty(LoginPassWord) && Common.DecryptData(objUserMaster.Password) != LoginPassWord)
                    {
                        ErrorMessage = "login information is incorrect";
                    }
                }
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ToastHelper.ShowError(ErrorMessage);
                    Error = true;
                    HasChanges = false;
                    LoginCommand.RaiseCanExecuteChanged();
                }
                if (objUserMaster != null)
                {
                    if (objUserMaster.TemplateID == 1)
                        Constant.IsAdmin = true;

                    if (objUserMaster.UserName == "svt_sauser" || objUserMaster.UserName == "endelsupport")
                    {
                        Constant.IsAdmin = true;
                        Constant.IsSupportUser = true;
                    }
                }
            }
            if (!HasErrors && !Error)
            {
                MainWindow mw = new MainWindow(objUserMaster.UserID);
                mw.Show();
                parentPage.Close();
            }
            else
                LoginCommand.RaiseCanExecuteChanged();
        }
        public string LoginLogo
        {
            get { return dataRepository.Context.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "CompanySetting" && x.SettingName == "Logo").SettingValue; }
            set { }
        }

        private string _LoginUserName;
        [Required(ErrorMessage = "User Name is required")]
        public string LoginUserName
        {
            get { return _LoginUserName; }
            set
            {
                _LoginUserName = value;
                if (LoginUserName == "")
                    HasChanges = false;
                else
                    HasChanges = true;
                objUserMaster = dataRepository.Context.UserMasters.FirstOrDefault(x => x.UserName.ToLower() == LoginUserName.ToLower()); LoginCommand.RaiseCanExecuteChanged(); ValidatePropertyInternal("LoginUserName", value);
            }
        }
        private bool _hasChanges;
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                    _hasChanges = value; OnPropertyChanged(); LoginCommand.RaiseCanExecuteChanged();
            }
        }
        private string _LoginPassWord;
        [Required(ErrorMessage = "Password is required")]
        public string LoginPassWord
        {
            get { return _LoginPassWord; }
            set
            {
                _LoginPassWord = value;
                if (LoginPassWord == "")
                    HasChanges = false;
                else
                    HasChanges = true;
                OnPropertyChanged(); LoginCommand.RaiseCanExecuteChanged(); ValidatePropertyInternal("LoginPassWord", _LoginPassWord);
            }
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
    }
}
