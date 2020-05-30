using CosmosDBStudio.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CosmosDBStudio.Services.Implementation
{
    public class QueryPersistenceService : IQueryPersistenceService
    {
        public QuerySheet Load(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<QuerySheet>(json);
        }

        public IList<string> LoadMruList()
        {
            try
            {
                string json = File.ReadAllText(GetMruListFilePath());
                return JsonConvert.DeserializeObject<IList<string>>(json);
            }
            catch(FileNotFoundException)
            {
                return new List<string>();
            }
        }

        public void Save(QuerySheet querySheet, string path)
        {
            var json = JsonConvert.SerializeObject(querySheet, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public void SaveMruList(IList<string> mruList)
        {
            string json = JsonConvert.SerializeObject(mruList, Formatting.Indented);
            File.WriteAllText(GetMruListFilePath(), json);
        }

        private static string GetMruListFilePath(bool createDirectory = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var cosmosDbStudioData = Path.Combine(appData, "CosmosDBStudio");
            if (createDirectory)
                Directory.CreateDirectory(cosmosDbStudioData);
            var filePath = Path.Combine(cosmosDbStudioData, "mru.json");
            return filePath;
        }
    }
}
