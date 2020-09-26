namespace CosmosDBStudio.Model
{
    public class CosmosDatabase : ICosmosItem
    {
        public string Id { get; set; } = string.Empty;
        public string? ETag { get; set; }
        public int? Throughput { get; set; }
    }
}