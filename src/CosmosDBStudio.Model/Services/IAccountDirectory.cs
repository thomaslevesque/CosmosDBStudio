using System.Collections.Generic;
using CosmosDBStudio.Model.Services.Implementation;

namespace CosmosDBStudio.Model.Services
{
    public interface IAccountDirectory
    {
        IEnumerable<CosmosAccount> Accounts { get; }
        void Add(CosmosAccount account);
        void Remove(string id);
        void Update(CosmosAccount account);
        void Load();
        void Save();
        bool TryGetById(string id, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out CosmosAccount? account);
        IEnumerable<ITreeNode> GetRootNodes();
        IEnumerable<ITreeNode> GetChildNodes(string folderPrefix);
    }
}
