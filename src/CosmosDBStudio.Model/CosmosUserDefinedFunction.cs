namespace CosmosDBStudio.Model
{
    public class CosmosUserDefinedFunction : ICosmosScript, ITreeNode
    {
        public string Id { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? ETag { get; set; } = string.Empty;

        string ITreeNode.Name => Id;

        public ICosmosScript Clone() => (ICosmosScript)MemberwiseClone();
    }
}
