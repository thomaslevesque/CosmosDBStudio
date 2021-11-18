using CosmosDBStudio.Model;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.Messages
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
