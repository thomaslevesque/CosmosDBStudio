using CosmosDBStudio.Model;

namespace CosmosDBStudio.Services
{
    public interface IAccountContextFactory
    {
        IAccountContext Create(CosmosAccount account);
    }
}
