using CosmosDBStudio.Model;
using EssentialMVVM;
using Newtonsoft.Json;

namespace CosmosDBStudio.ViewModel.EditorTabs
{
    public class StoredProcedureResultViewModel : BindableBase
    {
        public StoredProcedureResultViewModel(StoredProcedureResult result)
        {
            Text = result.Body?.ToString(Formatting.Indented) ?? string.Empty;
            ScriptLog = result.ScriptLog;
            Error = result.Error?.Message ?? string.Empty;
            if (result.Error != null)
                SelectedTab = ResultTab.Error;
        }

        public string Text { get; }
        public string ScriptLog { get; }
        public string Error { get; set; }

        private ResultTab _selectedTab;
        public ResultTab SelectedTab
        {
            get => _selectedTab;
            set => Set(ref _selectedTab, value)
                .AndNotifyPropertyChanged(nameof(IsScriptLogTabSelected))
                .AndNotifyPropertyChanged(nameof(IsRawTabSelected))
                .AndNotifyPropertyChanged(nameof(IsErrorTabSelected));
        }

        public bool IsRawTabSelected => SelectedTab == ResultTab.Raw;
        public bool IsScriptLogTabSelected => SelectedTab == ResultTab.ScriptLog;
        public bool IsErrorTabSelected => SelectedTab == ResultTab.Error;

        public enum ResultTab
        {
            Raw = 0,
            ScriptLog = 1,
            Error = 2
        }
    }
}
