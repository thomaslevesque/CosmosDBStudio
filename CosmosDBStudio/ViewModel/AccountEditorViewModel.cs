using CosmosDBStudio.Dialogs;
using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel
{
    public class AccountEditorViewModel : DialogViewModelBase
    {
        public AccountEditorViewModel(CosmosAccount account = null)
        {
            _name = account?.Name ?? string.Empty;
            _endpoint = account?.Endpoint ?? string.Empty;
            _key = account?.Key ?? string.Empty;

            Title = account is null
                ? "Add Cosmos DB account"
                : "Edit Cosmos DB account";

            AddOkButton();
            AddCancelButton();
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _endpoint;
        public string Endpoint
        {
            get => _endpoint;
            set => Set(ref _endpoint, value);
        }

        private string _key;
        public string Key
        {
            get => _key;
            set => Set(ref _key, value);
        }
    }
}
