using CosmosDBStudio.Model;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services.Implementation
{
    public class QueryPersistenceService : IQueryPersistenceService
    {
        public async Task<QuerySheet> LoadAsync(string path)
        {
            string json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<QuerySheet>(json);
        }

        public async Task SaveAsync(QuerySheet querySheet, string path)
        {
            var json = JsonConvert.SerializeObject(querySheet);
            await File.WriteAllTextAsync(path, json);
        }
    }
}
