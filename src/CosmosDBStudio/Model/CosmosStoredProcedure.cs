namespace CosmosDBStudio.Model
{
    public class CosmosStoredProcedure : ICosmosItem
    {
        public string Id { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string ETag { get; set; } = string.Empty;
    }
}
