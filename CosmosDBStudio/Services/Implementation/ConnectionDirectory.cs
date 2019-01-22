using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CosmosDBStudio.Model;
using Newtonsoft.Json;

namespace CosmosDBStudio.Services.Implementation
{
    class ConnectionDirectory : IConnectionDirectory
    {
        private Dictionary<string, DatabaseConnection> _connections;

        public ConnectionDirectory()
        {
            Load();
        }

        public IEnumerable<DatabaseConnection> Connections => _connections.Values.Select(c => c.Clone());

        public DatabaseConnection GetById(string id)
        {
            return _connections.TryGetValue(id, out var value) ? value.Clone() : null;
        }

        public void Add(DatabaseConnection connection)
        {
            _connections.Add(connection.Id, connection.Clone());
        }

        public void Remove(string id)
        {
            _connections.Remove(id);
        }

        public void Update(DatabaseConnection connection)
        {
            var existing = GetById(connection.Id);
            if (existing == null)
            {
                Add(connection.Clone());
            }
            else
            {
                existing.Endpoint = connection.Endpoint;
                existing.Name = connection.Name;
                existing.Key = connection.Key;
            }
        }

        public void Load()
        {
            try
            {
                var json = File.ReadAllText(GetConnectionsFilePath());
                var connections = JsonConvert.DeserializeObject<DatabaseConnection[]>(json);
                _connections = connections.ToDictionary(c => c.Id);
                return;
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }

            _connections = new Dictionary<string, DatabaseConnection>();
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(_connections.Values);
            File.WriteAllText(GetConnectionsFilePath(true), json);
        }

        private static string GetConnectionsFilePath(bool createDirectory = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var cosmosDbStudioData = Path.Combine(appData, "CosmosDBStudio");
            if (createDirectory)
                Directory.CreateDirectory(cosmosDbStudioData);
            var filePath = Path.Combine(cosmosDbStudioData, "connections.json");
            return filePath;
        }
    }
}