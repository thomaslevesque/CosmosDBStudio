namespace CosmosDBStudio.Messages
{
    public class OpenScriptMessage<TScript>
    {
        public OpenScriptMessage(string accountId, string databaseId, string containerId, TScript script)
        {
            AccountId = accountId;
            DatabaseId = databaseId;
            ContainerId = containerId;
            Script = script;
        }

        public string AccountId { get; }
        public string DatabaseId { get; }
        public string ContainerId { get; }
        public TScript Script { get; }
    }
}
