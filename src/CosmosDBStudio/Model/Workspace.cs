using System.Collections.Generic;

namespace CosmosDBStudio.Model
{
    public class Workspace
    {
        public int UntitledCounter { get; set; }
        public IList<WorkspaceQuerySheet> QuerySheets { get; set; } = new List<WorkspaceQuerySheet>();
    }

    public class WorkspaceQuerySheet
    {
        public string Title { get; set; } = string.Empty;
        public string? SavedPath { get; set; }
        public string? TempPath { get; set; }
        public bool HasChanges { get; set; }
        public bool IsCurrent { get; set; }
    }
}
