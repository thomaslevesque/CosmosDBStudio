namespace CosmosDBStudio.Model
{
    public class CosmosAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public string Key { get; set; }

        public CosmosAccount Clone()
        {
            return new CosmosAccount
            {
                Id = Id,
                Name = Name,
                Endpoint = Endpoint,
                Key = Key
            };
        }
    }
}