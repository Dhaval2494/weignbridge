using System.Data.Entity;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    class vmGateManagement : DataSetupEditViewModelBase<Gate>
    {
        public vmGateManagement(int GateID)
        {
            Id = GateID;
        }

        public override async Task LoadAsync()
        {
            var gate = Id > 0
              ? await dataRepository.Context.Gates.SingleAsync(x => x.GateID == Id)
              : CreateNewGate();

            InitializeModel(gate);
        }
        private Gate CreateNewGate()
        {
            var gate = CreateNewModel();
            gate.IsActive = true;
            return gate;
        }
        protected override void AfterSaveExecute()
        {
            ToastHelper.ShowSuccess("Gate information recorded successfully");
            RaiseDetailSavedEvent(Model.GateID, Model);
        }
    }
}