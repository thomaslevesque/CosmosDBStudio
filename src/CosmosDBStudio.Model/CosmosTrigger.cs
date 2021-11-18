using Microsoft.Azure.Cosmos.Scripts;

namespace CosmosDBStudio.Model
{
    public class CosmosTrigger : ICosmosScript
    {
        public string Id { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? ETag { get; set; }
        public TriggerOperation Operation { get; set; }
        public TriggerType Type { get; set; }

        public ICosmosScript Clone() => (ICosmosScript)MemberwiseClone();
    }
}
