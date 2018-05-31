using Newtonsoft.Json;

namespace CosmosDBStudio.Model
{
    public class DatabaseConnection
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public string Key { get; set; }

        public DatabaseConnection Clone()
        {
            return new DatabaseConnection
            {
                Id = Id,
                Name = Name,
                Endpoint = Endpoint,
                Key = Key
            };
        }
    }
}