using CosmosDBStudio.Model.Services.Implementation;

namespace CosmosDBStudio.Model
{
    public class CosmosAccount : ITreeNode
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public bool IsServerless { get; set; }
        public string Folder { get; set; } = string.Empty;
        
        string ITreeNode.DisplayName => Name;

        public CosmosAccount Clone()
        {
            return new CosmosAccount
            {
                Id = Id,
                Name = Name,
                Endpoint = Endpoint,
                Key = Key,
                IsServerless = IsServerless,
                Folder = Folder
            };
        }
    }
}