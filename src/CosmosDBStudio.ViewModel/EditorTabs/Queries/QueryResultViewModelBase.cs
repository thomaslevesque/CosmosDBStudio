using System.Collections.Generic;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel.EditorTabs.Queries
{
    public abstract class QueryResultViewModelBase : BindableBase
    {
        public abstract IReadOnlyList<ResultItemViewModel> Items { get; }
        public abstract bool IsJson { get; }
        public abstract string Text { get; }
        public abstract string? Error { get; }
        public abstract ResultItemViewModel? SelectedItem { get; set; }


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