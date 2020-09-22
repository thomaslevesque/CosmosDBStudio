using System.Collections.Generic;

namespace CosmosDBStudio.Model
{
    public class QuerySheet
    {
        public const string FileFilter = "Cosmos DB Studio query sheet|*.cdbsqs";

        public string Text { get; set; } = string.Empty;
        public string? PartitionKey { get; set; }
        public IList<string> PartitionKeyMRU { get; set; } = new List<string>();
        public IList<QuerySheetParameter> Parameters { get; set; } = new List<QuerySheetParameter>();
    }

    public class QuerySheetParameter
    {
        public string Name { get; set; } = string.Empty;
        public string? RawValue { get; set; }
        public IList<string> MRU { get; set; } = new List<string>();
    }
}
