namespace CosmosDBStudio.Model
{
    public class CosmosContainer : ICosmosItem, ITreeNode
    {
        public string Id { get; set; } = string.Empty;
        public string? ETag { get; set; }
        public string PartitionKeyPath { get; set; } = string.Empty;
        public bool LargePartitionKey { get; set; }
        public int? DefaultTTL { get; set; }

        string ITreeNode.DisplayName => Id;
    }
}