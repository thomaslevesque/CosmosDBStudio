namespace CosmosDBStudio.Model.Services
{
    public interface IAccountContextFactory
    {
        IAccountContext Create(CosmosAccount account);
    }
}
