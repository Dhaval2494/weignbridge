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
    class vmGateManagementList : DataSetupListViewModelBase<Gate>
    {
        public DelegateCommand<Gate> ActiveDeActiveGateCommand { get; private set; }
        public vmGateManagementList()
        {
            ActiveDeActiveGateCommand = new DelegateCommand<Gate>(OnActiveDeActiveGateCommand);
        }
        public async void OnActiveDeActiveGateCommand(Gate objGate)
        {
            DisallowtodeactivemasterEventArgs ObjAllowtoDeactive = new DisallowtodeactivemasterEventArgs();
            ObjAllowtoDeactive.ObjectID = objGate.GateID;
            ObjAllowtoDeactive.Objectinstance = objGate;
            if (objGate.IsActive)
            {
                EventAggregator.GetEvent<DisallowtodeactivemasterEvent>().Publish(ObjAllowtoDeactive);
                if (dataRepository.Context.TransactionDatas.Any(x => x.TransactionDetails.Any(y => y.GateID == objGate.GateID) && x.Status == TransactionStatus.Active.ToString()))
                    ObjAllowtoDeactive.Message = "it use in active transaction";
            }
            if (!string.IsNullOrEmpty(ObjAllowtoDeactive.Message))
                await MessageBoxHelper.ShowInfoDialogAsync("Warning", "You can not deactive, " + ObjAllowtoDeactive.Message);
            else
            {
                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objGate.IsActive ? "deactivate" : "activate") + " this gate?");
                if (msgbox_result == MessageDialogResult.Affirmative)
                {
                    objGate.IsActive = !objGate.IsActive;
                    await dataRepository.SaveAsync();
                    ViewList.View.Refresh();
                }
            }
        }


        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<Gate>(await dataRepository.Context.Gates.ToListAsync());
            CurrentPageIndex = 0;
            OnFilterData();
        }
        public override void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<Gate>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.GateName) && x.GateName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.GateType.ToString()) && x.GateType.ToString().ToLower().Contains(SearchString.ToLower()))).ToList());
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
