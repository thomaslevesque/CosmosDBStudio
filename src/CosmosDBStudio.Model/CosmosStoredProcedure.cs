namespace CosmosDBStudio.Model
{
    public class CosmosStoredProcedure : ICosmosScript, ITreeNode
    {
        public string Id { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? ETag { get; set; } = string.Empty;

        public string Name => Id;

        public ICosmosScript Clone() => (ICosmosScript)MemberwiseClone();
    }
}
