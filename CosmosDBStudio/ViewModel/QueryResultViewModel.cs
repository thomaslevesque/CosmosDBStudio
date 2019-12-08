using CosmosDBStudio.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CosmosDBStudio.ViewModel
{
    public class QueryResultViewModel : QueryResultViewModelBase
    {
        private readonly QueryResult _result;
        private readonly IViewModelFactory _viewModelFactory;

        public QueryResultViewModel(QueryResult result, IViewModelFactory viewModelFactory)
        {
            _result = result;
            _viewModelFactory = viewModelFactory;
            if (result.Items != null && result.Items.Count > 0)
            {
                Items = result.Items.Select(_viewModelFactory.CreateDocumentViewModel).ToList();
                Text = new JArray(result.Items).ToString(Formatting.Indented);
                IsJson = true;
                SelectedTab = ResultTab.Items;
            }
            else
            {
                ResultItemViewModel item;
                if (_result.Error != null)
                {
                    item = _viewModelFactory.CreateErrorItemPlaceholder();
                    SelectedTab = ResultTab.Error;
                }
                else
                {
                    item = _viewModelFactory.CreateEmptyResultPlaceholder();
                    SelectedTab = ResultTab.Items;
                }

                Items = new[] { item };
                Text = item.Text;
                IsJson = false;
            }

            SelectedItem = Items.FirstOrDefault();
        }

        public override IReadOnlyList<ResultItemViewModel> Items { get; }
        public override bool IsJson { get; }
        public override string Text { get; }
        public override string? Error => _result?.Error?.Message;

        private ResultItemViewModel? _selectedItem;
        public override ResultItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
    }
}