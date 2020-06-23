using CosmosDBStudio.Model;

namespace CosmosDBStudio.Messages
{
    public class DatabaseCreatedMessage
    {
        public DatabaseCreatedMessage(string accountId, CosmosDatabase database)
        {
            AccountId = accountId;
            Database = database;
        }

        public string AccountId { get; }
        public CosmosDatabase Database { get; }
    }
}
