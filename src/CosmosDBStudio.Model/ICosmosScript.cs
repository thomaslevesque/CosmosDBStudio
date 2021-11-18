namespace CosmosDBStudio.Model
{
    public interface ICosmosScript : ICosmosItem
    {
        string Body { get; set; }

        ICosmosScript Clone();
    }
}