using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;

namespace CosmosDBStudio.ViewModel.Messages
{
    public class DatabaseCreatedMessage
    {
        public DatabaseCreatedMessage(IAccountContext context, CosmosDatabase database)
        {
            Context = context;
            Database = database;
        }

        public IAccountContext Context { get; }
        public CosmosDatabase Database { get; }
    }
}
