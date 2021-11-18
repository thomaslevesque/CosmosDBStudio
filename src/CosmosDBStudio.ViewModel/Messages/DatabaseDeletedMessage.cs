using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;

namespace CosmosDBStudio.ViewModel.Messages
{
    public class DatabaseDeletedMessage
    {
        public DatabaseDeletedMessage(IAccountContext context, CosmosDatabase database)
        {
            Context = context;
            Database = database;
        }

        public IAccountContext Context { get; }
        public CosmosDatabase Database { get; }
    }
}
