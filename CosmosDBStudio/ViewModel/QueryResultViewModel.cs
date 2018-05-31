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
            if (result.Documents != null)
            {
                var array = new JArray(result.Documents);
                Json = array.ToString(Formatting.Indented);
            }

            IsErrorTabSelected = _result.Error != null;
        }

        public string Json { get; }
        public string Error => _result.Error?.ToString();

        private bool _isErrorTabSelected;
        public bool IsErrorTabSelected
        {
            get => _isErrorTabSelected;
            set => Set(ref _isErrorTabSelected, value);
        }
    }
}