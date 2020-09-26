using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.Messages
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
