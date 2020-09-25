namespace CosmosDBStudio.Model
{
    public interface ICosmosScript : ICosmosItem
    {
        string Body { get; set; }
        string? ETag { get; set; }

        ICosmosScript Clone();
    }
}