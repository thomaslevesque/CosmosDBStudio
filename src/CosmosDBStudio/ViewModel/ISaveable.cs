using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public interface ISaveable
    {
        void Save(string path);
        string? FilePath { get; }
        bool HasChanges { get; }
        string FileFilter { get; }
    }
}
