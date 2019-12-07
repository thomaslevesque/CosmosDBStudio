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
        private readonly IViewModelFactory _viewModelFactory;

        public QueryResultViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            SelectedTab = ResultTab.Items;
        }

        public QueryResultViewModel(QueryResult result, IViewModelFactory viewModelFactory)
        {
            _result = result;
            _viewModelFactory = viewModelFactory;
            if (result.Documents != null && result.Documents.Count > 0)
            {
                Items = result.Documents.Select(_viewModelFactory.CreateDocumentViewModel).ToList();
                Text = new JArray(result.Documents).ToString(Formatting.Indented);
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

        public IReadOnlyList<ResultItemViewModel> Items { get; }
        public bool IsJson { get; }
        public string Text { get; }
        public string Error => _result?.Error?.Message;

        private ResultItemViewModel _selectedItem;
        public ResultItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        private ResultTab _selectedTab;
        public ResultTab SelectedTab
        {
            get => _selectedTab;
            set => Set(ref _selectedTab, value)
                .AndNotifyPropertyChanged(nameof(IsItemsTabSelected))
                .AndNotifyPropertyChanged(nameof(IsRawTabSelected))
                .AndNotifyPropertyChanged(nameof(IsErrorTabSelected));
        }

        public bool IsItemsTabSelected => SelectedTab == ResultTab.Items;
        public bool IsRawTabSelected => SelectedTab == ResultTab.Raw;
        public bool IsErrorTabSelected => SelectedTab == ResultTab.Error;

        public enum ResultTab
        {
            Items = 0,
            Raw = 1,
            Error = 2
        }
    }
}