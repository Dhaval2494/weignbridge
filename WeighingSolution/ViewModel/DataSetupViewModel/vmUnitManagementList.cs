using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;

namespace WeighingSolution.ViewModel
{
    class vmUnitManagementList : DataSetupListViewModelBase<Unit>
    {
        public vmUnitManagementList()
        {
        }
        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<Unit>(await dataRepository.Context.Units.ToListAsync());
            CurrentPageIndex = 0;
            OnFilterData();
        }
        public override void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<Unit>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.UnitName) && x.UnitName.ToLower().Contains(SearchString.ToLower()))).ToList());
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
