using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WeighingSolution.DataLayer;
using System.Data.Entity;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Prism.Commands;
using MahApps.Metro.Controls.Dialogs;
using WeighingSolution.Helpers;
using WeighingSolution.Events;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class vmUHFManagementList : DataSetupListViewModelBase<UHFBindingListModel>
    {
        //public bool IsDeletePermission { get; set; }
        public DelegateCommand<UHFBindingListModel> ActiveDeActiveUHFCommand { get; private set; }
        //public DelegateCommand<UHFBindingListModel> DeleteUHFCommand { get; private set; }
        public vmUHFManagementList()
        {
            ActiveDeActiveUHFCommand = new DelegateCommand<UHFBindingListModel>(OnActiveDeActiveUHFCommand);
            //DeleteUHFCommand = new DelegateCommand<UHFBindingListModel>(OnDeleteUHFCommand);
        }

        public async void OnActiveDeActiveUHFCommand(UHFBindingListModel objUHF)
        {
            DisallowtodeactivemasterEventArgs ObjAllowtoDeactive = new DisallowtodeactivemasterEventArgs();
            ObjAllowtoDeactive.ObjectID = objUHF.ID;
            ObjAllowtoDeactive.Objectinstance = objUHF;
            if (objUHF.IsActive)
            {
                EventAggregator.GetEvent<DisallowtodeactivemasterEvent>().Publish(ObjAllowtoDeactive);
            }
            if (!string.IsNullOrEmpty(ObjAllowtoDeactive.Message))
                await MessageBoxHelper.ShowInfoDialogAsync("Warning", "You can not deactive, " + ObjAllowtoDeactive.Message);
            else
            {
                if (!objUHF.IsActive)
                {
                    var uhfMasters = await dataRepository.Context.UhfMasters.Where(x => x.IsActive).ToListAsync();
                    var isAllow = uhfMasters.Any(x => x.WeightbridgeSide.ToLower() == objUHF.WeightbridgeSide.ToLower());
                    if (isAllow)
                    {
                        ToastHelper.ShowError($"UHF with weightbridge side {objUHF.WeightbridgeSide} is already exist. so you can not active this UHF");
                        return;
                    }
                }
                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objUHF.IsActive ? "deactivate" : "activate") + " this UHF?");
                if (msgbox_result == MessageDialogResult.Affirmative)
                {
                    UhfMaster _UhfMaster = dataRepository.Context.UhfMasters.Find(objUHF.ID);
                    _UhfMaster.IsActive = !objUHF.IsActive;
                    objUHF.IsActive = !objUHF.IsActive;
                    await dataRepository.SaveAsync();
                    ViewList.View.Refresh();
                }
            }
        }

        //public async void OnDeleteUHFCommand(UHFBindingListModel objUHF)
        //{
        //    if(Common.ValidatePageRequest(PageEntity.UHF, PageAccess.Delete))
        //    {
        //        MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to delete this UHF?");
        //        if (msgbox_result == MessageDialogResult.Affirmative)
        //        {
        //            UhfMaster uhfMaster = await dataRepository.Context.UhfMasters.FirstOrDefaultAsync(x => x.ID == objUHF.ID);
        //            UhfMaster _UhfMaster = dataRepository.Context.UhfMasters.Remove(uhfMaster);
        //            await dataRepository.SaveAsync();
        //            ViewList.View.Refresh();
        //        }
        //    }
        //}

        public async Task LoadAsync()
        {
            //IsDeletePermission = Common.ValidatePageRequest(PageEntity.UHF, PageAccess.Delete);
            AllObjectList = new ObservableCollection<UHFBindingListModel>(await dataRepository.Context.UhfMasters.Select(x => new UHFBindingListModel
            {
                ID = x.ID,
                IpAddress = x.IpAddress,
                Port = x.Port,
                IsActive = x.IsActive,
                WeightbridgeID = x.WeightbridgeID,
                ScaleName = x.WeightBridge.ScaleName,
                WeightbridgeSide = x.WeightbridgeSide
            }).OrderBy(p => p.ID).ToListAsync());
            ;
            OnFilterData();
        }
        public override void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<UHFBindingListModel>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.ScaleName) && x.ScaleName.ToLower().Contains(SearchString.ToLower())) || !string.IsNullOrEmpty(x.IpAddress) && x.IpAddress.ToLower().Contains(SearchString.ToLower())).ToList());
            }
            else
            {
                FilteredObjectList = AllObjectList;
            }
            CurrentPageIndex = 0;
            CalculateTotalPages();
            ViewList.Source = FilteredObjectList;
            ViewList.View.Refresh();
        }
    }
}
