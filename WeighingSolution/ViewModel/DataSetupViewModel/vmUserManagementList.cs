using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    class vmUserManagementList : DataSetupListViewModelBase<UserMaster>
    {
        public DelegateCommand<UserMaster> ActiveDeActiveUserCommand { get; private set; }

        public vmUserManagementList()
        {
            ActiveDeActiveUserCommand = new DelegateCommand<UserMaster>(OnActiveDeActiveUserCommand);
        }

        public async void OnActiveDeActiveUserCommand(UserMaster objuser)
        {
            if (Convert.ToInt32(Application.Current.Properties["LoggedInUserID"]) == objuser.UserID)
            {
                ToastHelper.ShowWarning("You can not deactive logged in User");
                return;
            }
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objuser.Active ? "deactivate" : "activate") + " this user?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                objuser.Active = !objuser.Active;
                await dataRepository.SaveAsync();
                ViewList.View.Refresh();
            }
        }

        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<UserMaster>(await dataRepository.Context.UserMasters.Where(x => x.UserName.ToLower() != "svt_sauser" && x.UserName.ToLower() != "endelsupport").ToListAsync());
            CurrentPageIndex = 0;
            OnFilterData();
        }
        public override void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<UserMaster>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.FirstName) && x.FirstName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.LastName) && x.LastName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Contains(SearchString.ToLower()))).ToList());
            }
            else
            {
                FilteredObjectList = AllObjectList;
            }
            //CurrentPageIndex = 0;
            CalculateTotalPages(FilteredObjectList.Count);
            ViewList.Source = FilteredObjectList.Skip(CurrentPageIndex * itemPerPage).Take(itemPerPage);
            ViewList.View.Refresh();
        }
    }
}
