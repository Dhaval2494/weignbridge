using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    class vmTemplateManagementList : DataSetupListViewModelBase<Template>
    {
        public DelegateCommand<Template> ActiveDeActiveUserRoleCommand { get; private set; }
        public vmTemplateManagementList()
        {
            ActiveDeActiveUserRoleCommand = new DelegateCommand<Template>(OnActiveDeActiveUserRoleCommand);
        }
        public async void OnActiveDeActiveUserRoleCommand(Template objTemplate)
        {
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objTemplate.Active ? "deactivate" : "activate") + " this user role?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                objTemplate.Active = !objTemplate.Active;
                await dataRepository.SaveAsync();
                ViewList.View.Refresh();
            }
        }
        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<Template>(await dataRepository.Context.Templates.Where(x => x.TemplateName.ToLower().Trim() != "admin").ToListAsync());
            CurrentPageIndex = 0;
            OnFilterData();
        }
        public override void OnFilterData()
        {
            FilteredObjectList = AllObjectList;
            //CurrentPageIndex = 0;
            CalculateTotalPages(FilteredObjectList.Count);
            ViewList.Source = FilteredObjectList.Skip(CurrentPageIndex * itemPerPage).Take(itemPerPage);
            ViewList.View.Refresh();
        }
    }
}
