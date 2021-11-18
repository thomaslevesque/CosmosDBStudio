using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CosmosDBStudio.Model.Services.Implementation
{
    public class QueryPersistenceService : IQueryPersistenceService
    {
        public QuerySheet? Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<QuerySheet>(json);
                }
                catch (FileNotFoundException) { }
                catch (DirectoryNotFoundException) { }
            }

            return null;
        }

        public IList<string> LoadMruList()
        {
            try
            {
                string json = File.ReadAllText(GetMruListFilePath());
                return JsonConvert.DeserializeObject<IList<string>>(json)
                    ?? new List<string>();
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }

            return new List<string>();
        }

        public void Save(QuerySheet querySheet, string path)
        {
            var json = JsonConvert.SerializeObject(querySheet, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public void SaveMruList(IList<string> mruList)
        {
            string json = JsonConvert.SerializeObject(mruList, Formatting.Indented);
            File.WriteAllText(GetMruListFilePath(), json);
        }

        public string SaveWorkspaceTempQuery(QuerySheet querySheet)
        {
            var tempPath = Path.GetTempFileName();
            var newPath = Path.Combine(GetWorkspacePath(true), Path.GetFileName(tempPath));
            File.Move(tempPath, newPath);
            Save(querySheet, newPath);
            return newPath;
        }

        public void SaveWorkspace(Workspace workspace)
        {
            var workspaceDir = GetWorkspacePath(true);
            var workspaceFile = Path.Combine(workspaceDir, "workspace.json");
            string json = JsonConvert.SerializeObject(workspace, Formatting.Indented);
            File.WriteAllText(workspaceFile, json);

            // Cleanup old temporary query sheets
            var oldTmpFiles = Directory.EnumerateFiles(workspaceDir, "*.tmp")
                .Except(workspace.QuerySheets.Select(s => s.TempPath ?? string.Empty))
                .ToList();
            
            foreach (var oldTmpFile in oldTmpFiles)
            {
                try
                {
                    File.Delete(oldTmpFile);
                }
                catch (Exception ex)
                {
                    Debug.Fail($"Failed to delete '{oldTmpFile}'", ex.ToString());
                }
            }
        }

        public Workspace LoadWorkspace()
        {

            var path = Path.Combine(GetWorkspacePath(false), "workspace.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    var workspace = JsonConvert.DeserializeObject<Workspace>(json);
                    if (workspace is not null)
                        return workspace;
                }
                catch (FileNotFoundException) { }
                catch (DirectoryNotFoundException) { }
            }

            return new Workspace();
        }

        private static string GetMruListFilePath(bool createDirectory = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var cosmosDbStudioData = Path.Combine(appData, "CosmosDBStudio");
            if (createDirectory)
                Directory.CreateDirectory(cosmosDbStudioData);
            var filePath = Path.Combine(cosmosDbStudioData, "mru.json");
            return filePath;
        }

        private static string GetWorkspacePath(bool createDirectory = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var cosmosDbStudioData = Path.Combine(appData, "CosmosDBStudio");
            var workspacePath = Path.Combine(cosmosDbStudioData, "workspace");
            if (createDirectory)
                Directory.CreateDirectory(workspacePath);
            return workspacePath;
        }
    }
}
