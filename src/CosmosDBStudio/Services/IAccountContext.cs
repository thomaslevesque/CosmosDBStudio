using CosmosDBStudio.Model;

namespace CosmosDBStudio.Services
{
    public interface IAccountContext
    {
        string AccountId { get; }
        string AccountName { get; }

        
        IDatabaseService Databases { get; }
        IDatabaseContext GetDatabaseContext(CosmosDatabase database);
    }
}
