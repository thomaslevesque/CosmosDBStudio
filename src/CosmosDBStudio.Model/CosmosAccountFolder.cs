using System.IO;
using CosmosDBStudio.Model.Services.Implementation;

namespace CosmosDBStudio.Model
{
    public class CosmosAccountFolder : ITreeNode
    {
        public CosmosAccountFolder(string fullPath)
        {
            FullPath = fullPath;
        }

        public string Name => Path.GetFileName(FullPath);
        public string FullPath { get; set; }
    }
}