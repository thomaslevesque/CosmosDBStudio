namespace CosmosDBStudio.Messages
{
    public class NewQuerySheetMessage
    {
        public NewQuerySheetMessage(string connectionId, string databaseId, string containerId)
        {
            ConnectionId = connectionId;
            DatabaseId = databaseId;
            ContainerId = containerId;
        }

        public string ConnectionId { get; }
        public string DatabaseId { get; }
        public string ContainerId { get; }
    }
}
