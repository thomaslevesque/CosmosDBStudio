using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CosmosDBStudio.Model;
using Newtonsoft.Json;

namespace CosmosDBStudio.Services.Implementation
{
    class AccountDirectory : IAccountDirectory
    {
        private Dictionary<string, CosmosAccount> _accounts;

        public AccountDirectory()
        {
            Load();
        }

        public IEnumerable<CosmosAccount> Accounts => _accounts.Values.Select(c => c.Clone());

        public CosmosAccount GetById(string id) => GetById(id, true);

        private CosmosAccount GetById(string id, bool clone)
        {
            return _accounts.TryGetValue(id, out var value) ? (clone ? value.Clone() : value) : null;
        }

        public void Add(CosmosAccount account)
        {
            _accounts.Add(account.Id, account.Clone());
        }

        public void Remove(string id)
        {
            _accounts.Remove(id);
        }

        public void Update(CosmosAccount account)
        {
            var existing = GetById(account.Id, false);
            if (existing == null)
            {
                Add(account);
            }
            else
            {
                existing.Endpoint = account.Endpoint;
                existing.Name = account.Name;
                existing.Key = account.Key;
            }
        }

        public void Load()
        {
            try
            {
                var json = File.ReadAllText(GetAccountsFilePath());
                var accounts = JsonConvert.DeserializeObject<CosmosAccount[]>(json);
                _accounts = accounts.ToDictionary(c => c.Id);
                return;
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }

            _accounts = new Dictionary<string, CosmosAccount>();
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(_accounts.Values);
            File.WriteAllText(GetAccountsFilePath(true), json);
        }

        private static string GetAccountsFilePath(bool createDirectory = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var cosmosDbStudioData = Path.Combine(appData, "CosmosDBStudio");
            if (createDirectory)
                Directory.CreateDirectory(cosmosDbStudioData);
            var filePath = Path.Combine(cosmosDbStudioData, "accounts.json");
            return filePath;
        }
    }
}