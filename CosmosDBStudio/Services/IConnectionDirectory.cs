using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDBStudio.Model;

namespace CosmosDBStudio.Services
{
    public interface IConnectionDirectory
    {
        IEnumerable<DatabaseConnection> Connections { get; }
        DatabaseConnection GetById(string id);
        void Add(DatabaseConnection connection);
        void Remove(string id);
        void Update(DatabaseConnection connection);
        void Load();
        void Save();
    }
}
