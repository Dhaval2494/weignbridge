using Prism.Commands;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    class vmTemplateManagement : DataSetupEditViewModelBase<Template>
    {
        public DelegateCommand<TemplatePermission> ReadCheckChangedCommand { get; private set; }
        public DelegateCommand<object[]> CheckboxCheckedCommand { get; private set; }
        public vmTemplateManagement(int TemplateID)
        {
            Id = TemplateID;
            ReadCheckChangedCommand = new DelegateCommand<TemplatePermission>(OnReadCheckChanged);
            CheckboxCheckedCommand = new DelegateCommand<object[]>(OnCheckboxChecked);
        }

        public override async Task LoadAsync()
        {
            var template = Id > 0
              ? await dataRepository.Context.Templates.Include("TemplatePermissions.Feature").SingleAsync(x => x.TemplateID == Id)
              : await CreateNewTemplate();

            InitializeModel(template);
            InitializeModelPermissions(template.TemplatePermissions);
        }

        private void InitializeModelPermissions(ObservableCollection<TemplatePermission> templatePermissions)
        {
            foreach (var model in templatePermissions)
            {
                model.PropertyChanged -= Model_PropertyChanged;
                model.PropertyChanged += Model_PropertyChanged;
            }
        }

        private async Task<Template> CreateNewTemplate()
        {
            var template = CreateNewModel();
            template.Active = true;
            template.TemplatePermissions = new ObservableCollection<TemplatePermission>();
            var lstFeature = await dataRepository.Context.Features.ToListAsync();
            foreach (Feature item in lstFeature)
            {
                TemplatePermission objFeatureTemplate = new TemplatePermission();
                objFeatureTemplate.FeatureID = item.FeatureID;
                objFeatureTemplate.Feature = item;
                objFeatureTemplate.PermissionSet = "11111";
                template.TemplatePermissions.Add(objFeatureTemplate);
            }
            return template;
        }

        protected override void AfterSaveExecute()
        {
            Constant.lstPermission = null;
            ToastHelper.ShowSuccess("User Role information recorded successfully");
            RaiseDetailSavedEvent(Model.TemplateID, Model);
        }

        #region Checkbox Check / Uncheck Commands
        private void SetReadTrueIncludingParents(int? FeatureID)
        {
            if (FeatureID.HasValue)
            {
                var item = Model.TemplatePermissions.FirstOrDefault(x => x.FeatureID == FeatureID);
                item.Read = item.Feature.ReadVisible ? true : false;
                SetReadTrueIncludingParents(item.Feature.ParentFeatureID);
            }
        }

        private void SetPermissionsOfChildren(int FeatureID, bool PermissionValue)
        {
            var AllChildPerms = Model.TemplatePermissions.Where(x => x.Feature.ParentFeatureID == FeatureID);
            foreach (var item in AllChildPerms)
            {
                item.Create = item.Feature.CreateVisible ? PermissionValue : false;
                item.Read = item.Feature.ReadVisible ? PermissionValue : false;
                item.Update = item.Feature.UpdateVisible ? PermissionValue : false;
                item.Delete = item.Feature.DeleteVisible ? PermissionValue : false;
                item.Execute = item.Feature.ExecuteVisible ? PermissionValue : false;
                SetPermissionsOfChildren(item.FeatureID, PermissionValue);
            }
        }

        private void SetReadFalseIfNoChildPermissions(int? FeatureID)
        {
            if (FeatureID.HasValue)
            {
                var item = Model.TemplatePermissions.FirstOrDefault(x => x.FeatureID == FeatureID);
                var childItems = Model.TemplatePermissions.Where(x => x.Feature.ParentFeatureID == item.FeatureID).ToList();
                if (item.Create || item.Update || item.Delete || item.Execute ||
                    childItems.Any(x => x.Read || x.Create || x.Update || x.Delete || x.Execute))
                    return;

                item.Read = false;
                SetReadFalseIfNoChildPermissions(item.Feature.ParentFeatureID);
            }
        }

        private void OnCheckboxChecked(object[] param)
        {
            bool IsChecked = (bool)param[1];
            TemplatePermission obj = param[0] as TemplatePermission;

            //Any of CUDE is checked
            if (IsChecked)
            {
                //Set Read True for this feature as well as all parent features
                SetReadTrueIncludingParents(obj.FeatureID);
            }
            else if (obj.Read == false)
            {
                //If all child permissions are not set, then remove read of parent
                SetReadFalseIfNoChildPermissions(obj.Feature.ParentFeatureID);
            }
        }

        private void OnReadCheckChanged(TemplatePermission obj)
        {
            if (obj.Read)
            {
                //Give Full Permission to all children
                SetPermissionsOfChildren(obj.FeatureID, true);

                //Give Read Permission to all parents
                SetReadTrueIncludingParents(obj.Feature.ParentFeatureID);
            }
            else
            {
                //Remove All Child Permissions
                SetPermissionsOfChildren(obj.FeatureID, false);

                //Remove all CRUDE Permissions for the object
                obj.Create = false;
                obj.Update = false;
                obj.Delete = false;
                obj.Execute = false;

                //If all child permissions are not set, then remove read of parent
                SetReadFalseIfNoChildPermissions(obj.Feature.ParentFeatureID);
            }
        }
        #endregion
    }
}