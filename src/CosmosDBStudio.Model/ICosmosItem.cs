namespace CosmosDBStudio.Model
{
    public interface ICosmosItem
    {
        string Id { get; set; }
        string? ETag { get; set; }
    }
}