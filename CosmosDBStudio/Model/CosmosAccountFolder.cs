using System.IO;

namespace CosmosDBStudio.Model
{
    public class CosmosAccountFolder
    {
        public CosmosAccountFolder(string fullPath)
        {
            FullPath = fullPath;
        }

        public string Name => Path.GetFileName(FullPath);
        public string FullPath { get; set; }
    }
}