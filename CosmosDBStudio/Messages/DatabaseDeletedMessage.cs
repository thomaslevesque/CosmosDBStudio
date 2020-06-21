using CosmosDBStudio.Model;

namespace CosmosDBStudio.Messages
{
    public class DatabaseDeletedMessage
    {
        public DatabaseDeletedMessage(string accountId, CosmosDatabase database)
        {
            AccountId = accountId;
            Database = database;
        }

        public string AccountId { get; }
        public CosmosDatabase Database { get; }
    }
}
