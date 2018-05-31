using CosmosDBStudio.Model;
using EssentialMVVM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                var array = new JArray(result.Documents);
                Json = array.ToString(Formatting.Indented);
            }
            else
            {
                if (_result.Error != null)
                    Json = "(Error - see Error tab for details)";
                else
                    Json = "(no results)";
            }

            IsErrorTabSelected = _result.Error != null;
        }

        public string Json { get; }
        public string Error => _result.Error?.ToString();

        public bool IsResultsTabSelected => !IsErrorTabSelected;

        private bool _isErrorTabSelected;
        public bool IsErrorTabSelected
        {
            get => _isErrorTabSelected;
            set => Set(ref _isErrorTabSelected, value).AndNotifyPropertyChanged(nameof(IsResultsTabSelected));
        }
    }
}