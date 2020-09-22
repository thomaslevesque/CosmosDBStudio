namespace CosmosDBStudio.Model
{
    public class CosmosContainer : ICosmosItem
    {
        public string Id { get; set; } = string.Empty;
        public string PartitionKeyPath { get; set; } = string.Empty;
        public bool LargePartitionKey { get; set; }
        public int? Throughput { get; set; }
        public int? DefaultTTL { get; set; }
    }
}