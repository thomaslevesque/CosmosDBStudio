namespace CosmosDBStudio.Messages
{
    public class NewQuerySheetMessage
    {
        public NewQuerySheetMessage(string connectionId, string databaseId, string collectionId)
        {
            ConnectionId = connectionId;
            DatabaseId = databaseId;
            CollectionId = collectionId;
        }

        public string ConnectionId { get; }
        public string DatabaseId { get; }
        public string CollectionId { get; }
    }
}
