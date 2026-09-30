using System.Data.Entity;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    class vmProductManagement : DataSetupEditViewModelBase<Product>
    {
        public vmProductManagement(int ProductID)
        {
            Id = ProductID;
        }

        public override async Task LoadAsync()
        {
            var product = Id > 0
              ? await dataRepository.Context.Products.SingleAsync(x => x.ProductID == Id)
              : CreateNewProduct();

            InitializeModel(product);
        }
        private Product CreateNewProduct()
        {
            var product = CreateNewModel();
            product.IsActive = true;
            return product;
        }
        protected override void AfterSaveExecute()
        {
            ToastHelper.ShowSuccess("Product information recorded successfully");
            RaiseDetailSavedEvent(Model.ProductID, Model);
        }
    }
}