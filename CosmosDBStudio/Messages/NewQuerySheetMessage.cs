namespace CosmosDBStudio.Messages
{
    public class NewQuerySheetMessage
    {
        public NewQuerySheetMessage(string accountId, string databaseId, string containerId)
        {
            AccountId = accountId;
            DatabaseId = databaseId;
            ContainerId = containerId;
        }

        public string AccountId { get; }
        public string DatabaseId { get; }
        public string ContainerId { get; }
    }
}
