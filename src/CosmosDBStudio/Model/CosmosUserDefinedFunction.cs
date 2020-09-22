namespace CosmosDBStudio.Model
{
    public class CosmosUserDefinedFunction : ICosmosItem
    {
        public string Id { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string ETag { get; set; } = string.Empty;
    }
}
