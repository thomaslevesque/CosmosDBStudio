namespace CosmosDBStudio.Model
{
    public class CosmosAccount
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Folder { get; set; } = string.Empty;

        public CosmosAccount Clone()
        {
            return new CosmosAccount
            {
                Id = Id,
                Name = Name,
                Endpoint = Endpoint,
                Key = Key,
                Folder = Folder
            };
        }
    }
}