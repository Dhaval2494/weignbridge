using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    class vmWeighBridgeManagementList : DataSetupListViewModelBase<WeightBridge>
    {
        public DelegateCommand<WeightBridge> ActiveDeActiveAccountCommand { get; private set; }
        public DelegateCommand<WeightBridge> DeleteCommand { get; private set; }
        public vmWeighBridgeManagementList()
        {
            ActiveDeActiveAccountCommand = new DelegateCommand<WeightBridge>(OnActiveDeActiveAccountCommand);
            DeleteCommand = new DelegateCommand<WeightBridge>(OnDeleteCommand);
        }

        private async void OnDeleteCommand(WeightBridge objWeighBridge)
        {
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to delete this WeighBridge?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                dataRepository.Context.WeightBridges.Remove(objWeighBridge);
                await dataRepository.SaveAsync();
                await LoadAsync();
                ViewList.View.Refresh();
            }
        }

        public async void OnActiveDeActiveAccountCommand(WeightBridge objWeighBridge)
        {
            if (objWeighBridge.IsActive && dataRepository.Context.WeightBridges.Count(x => x.IsActive) == 1)
            {
                await MessageBoxHelper.ShowInfoDialogAsync("Warning", "You can not deactive all weighbridge(s).");
            }
            DisallowtodeactivemasterEventArgs ObjAllowtoDeactive = new DisallowtodeactivemasterEventArgs();
            ObjAllowtoDeactive.ObjectID = objWeighBridge.WeightBridgeID;
            ObjAllowtoDeactive.Objectinstance = objWeighBridge;
            if (objWeighBridge.IsActive)
                EventAggregator.GetEvent<DisallowtodeactivemasterEvent>().Publish(ObjAllowtoDeactive);
            if (!string.IsNullOrEmpty(ObjAllowtoDeactive.Message))
                await MessageBoxHelper.ShowInfoDialogAsync("Warning", "You can not deactive, " + ObjAllowtoDeactive.Message);
            else
            {
                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objWeighBridge.IsActive ? "deactivate" : "activate") + " this WeighBridge?");
                if (msgbox_result == MessageDialogResult.Affirmative)
                {
                    objWeighBridge.IsActive = !objWeighBridge.IsActive;
                    await dataRepository.SaveAsync();
                    ViewList.View.Refresh();
                }
            }
        }
        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<WeightBridge>(await dataRepository.Context.WeightBridges.ToListAsync());
            CurrentPageIndex = 0;
            OnFilterData();
        }
        public override void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<WeightBridge>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.ScaleName) && x.ScaleName.ToLower().Contains(SearchString.ToLower()) || (!string.IsNullOrEmpty(x.ConnectionType) && x.ConnectionType.ToLower().Contains(SearchString.ToLower())))).ToList());
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
