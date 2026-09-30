using System.Threading.Tasks;

namespace WeighingSolution.DataLayer
{
    public class DataRepository
    {
        public readonly weighbridgeEntities Context;

        public DataRepository(weighbridgeEntities context)
        {
            this.Context = context;
        }

        public bool HasChanges()
        {
            return Context.ChangeTracker.HasChanges();
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }

        public void Save()
        {
            Context.SaveChanges();
        }

    }
}
