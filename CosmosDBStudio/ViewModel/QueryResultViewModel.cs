using CosmosDBStudio.Model;
using EssentialMVVM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CosmosDBStudio.ViewModel
{
    public class QueryResultViewModel : BindableBase
    {
        private readonly QueryResult _result;

        public QueryResultViewModel(QueryResult result)
        {
            _result = result;
            if (result.Documents != null && result.Documents.Count > 0)
            {
                Documents = result.Documents.Select(d => new DocumentViewModel(d)).ToList();
                Json = new JArray(result.Documents).ToString(Formatting.Indented);
                _selectedTab = ResultTab.Documents;
            }
            else
            {
                DocumentViewModel doc;
                if (_result.Error != null)
                {
                    doc = DocumentViewModel.Error();
                    _selectedTab = ResultTab.Error;
                }
                else
                {
                    doc = DocumentViewModel.NoResults();
                    _selectedTab = ResultTab.Documents;
                }

                Documents = new[] { doc };
                Json = doc.Id;
            }
        }

        public IReadOnlyList<DocumentViewModel> Documents { get; }
        public string Json { get; }
        public string Error => _result.Error?.Message;

        private ResultTab _selectedTab;
        public ResultTab SelectedTab
        {
            get => _selectedTab;
            set => Set(ref _selectedTab, value)
                .AndNotifyPropertyChanged(nameof(IsDocumentsTabSelected))
                .AndNotifyPropertyChanged(nameof(IsErrorTabSelected))
                .AndNotifyPropertyChanged(nameof(IsDocumentsTabSelected));
        }

        public bool IsDocumentsTabSelected => SelectedTab == ResultTab.Documents;
        public bool IsJsonTabSelected => SelectedTab == ResultTab.Json;
        public bool IsErrorTabSelected => SelectedTab == ResultTab.Error;

        public enum ResultTab
        {
            Documents = 0,
            Json = 1,
            Error = 2
        }
    }
}