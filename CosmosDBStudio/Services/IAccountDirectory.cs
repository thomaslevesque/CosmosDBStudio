using System.Collections.Generic;
using CosmosDBStudio.Model;

namespace CosmosDBStudio.Services
{
    public interface IAccountDirectory
    {
        IEnumerable<CosmosAccount> Accounts { get; }
        CosmosAccount GetById(string id);
        void Add(CosmosAccount account);
        void Remove(string id);
        void Update(CosmosAccount account);
        void Load();
        void Save();
    }
}
