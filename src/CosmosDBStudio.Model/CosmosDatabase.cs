namespace CosmosDBStudio.Model
{
    public class CosmosDatabase : ICosmosItem, ITreeNode
    {
        public string Id { get; set; } = string.Empty;
        public string? ETag { get; set; }

        string ITreeNode.DisplayName => Id;
    }
}