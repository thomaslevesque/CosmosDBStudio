using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using CosmosDBStudio.ViewModel;
using EssentialMVVM;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.Commands
{
    public class ScriptCommands<TScript>
        where TScript : ICosmosScript, new()
    {
        private readonly IMessenger _messenger;
        private readonly IDialogService _dialogService;

        public ScriptCommands(IMessenger messenger, IDialogService dialogService)
        {
            _messenger = messenger;
            _dialogService = dialogService;
        }

        #region Open

        private DelegateCommand<ScriptNodeViewModel<TScript>>? _openCommand;
        public ICommand OpenCommand => _openCommand ??= new DelegateCommand<ScriptNodeViewModel<TScript>>(Open);

        private void Open(ScriptNodeViewModel<TScript> scriptVm)
        {
            _messenger.Publish(new OpenScriptMessage<TScript>(scriptVm.Context, scriptVm.Script));
        }

        #endregion

        #region Create

        private DelegateCommand<ScriptFolderNodeViewModel>? _createCommand;
        public ICommand CreateCommand => _createCommand ??= new DelegateCommand<ScriptFolderNodeViewModel>(Create);

        private void Create(ScriptFolderNodeViewModel parent)
        {
            var result = _dialogService.TextPrompt("Enter id for new item");
            if (!result.TryGetValue(out var id))
            {
                return;
            }

            if (parent.Children.OfType<ScriptNodeViewModel<TScript>>().Any(c => c.Script.Id == id))
            {
                _dialogService.ShowError("Another item with the same name already exists");
                return;
            }

            var script = new TScript();
            script.Id = id;
            script.Body = $@"function {id} () {{
    
}}";

            _messenger.Publish(new OpenScriptMessage<TScript>(parent.Context, script));
        }

        #endregion

        #region Delete

        private AsyncDelegateCommand<ScriptNodeViewModel<TScript>>? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new AsyncDelegateCommand<ScriptNodeViewModel<TScript>>(DeleteAsync);

        private async Task DeleteAsync(ScriptNodeViewModel<TScript> scriptVm)
        {
            if (!_dialogService.Confirm($"Are you sure you want to delete {scriptVm.Description} '{scriptVm.Script.Id}'?"))
                return;

            await scriptVm.DeleteAsync();

            scriptVm.Parent?.ReloadChildren();
        }

        #endregion
    }
}
