namespace CosmosDBStudio.Model
{
    public class CosmosDatabase
    {
        public string Id { get; set; } = string.Empty;
        public int? Throughput { get; set; }
    }
}