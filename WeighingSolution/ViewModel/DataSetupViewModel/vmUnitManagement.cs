using System.Data.Entity;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    class vmUnitManagement : DataSetupEditViewModelBase<Unit>
    {
        public vmUnitManagement(int UnitID)
        {
            Id = UnitID;
        }

        public override async Task LoadAsync()
        {
            var unit = Id > 0
              ? await dataRepository.Context.Units.SingleAsync(x => x.UnitID == Id)
              : CreateNewModel();

            InitializeModel(unit);
        }
        protected override void AfterSaveExecute()
        {
            ToastHelper.ShowSuccess("Unit information recorded successfully");
            RaiseDetailSavedEvent(Model.UnitID, Model);
        }
    }
}