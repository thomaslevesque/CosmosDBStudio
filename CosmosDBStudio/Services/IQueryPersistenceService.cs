using CosmosDBStudio.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IQueryPersistenceService
    {
        QuerySheet Load(string path);
        void Save(QuerySheet querySheet, string path);
        IList<string> LoadMruList();
        void SaveMruList(IList<string> mruList);
    }
}
