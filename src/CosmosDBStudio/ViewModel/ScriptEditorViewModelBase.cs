using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public abstract class ScriptEditorViewModelBase<TScript> : TabViewModelBase
        where TScript : ICosmosScript
    {
        public ScriptEditorViewModelBase(IContainerContext containerContext, TScript script)
        {
            ContainerContext = containerContext;
            Script = script;
            Text = script.Body;
        }

        public override string Title => Script.Id;

        public IContainerContext ContainerContext { get; }
        public TScript Script { get; set; }

        public string ContainerPath => ContainerContext?.Path ?? "(no container selected)";

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value)
                .AndNotifyPropertyChanged(nameof(HasChanges))
                .AndExecute(RefreshCommands);
        }

        private string _selectedText = string.Empty;
        public string SelectedText
        {
            get => _selectedText;
            set => Set(ref _selectedText, value);
        }

        private (int start, int end) _selection;
        public (int start, int end) Selection
        {
            get => _selection;
            set => Set(ref _selection, value);
        }

        private int _cursorPosition;
        public int CursorPosition
        {
            get => _cursorPosition;
            set => Set(ref _cursorPosition, value);
        }

        private AsyncDelegateCommand? _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new AsyncDelegateCommand(SaveAsync, CanSave);

        private async Task SaveAsync()
        {
            var script = (TScript)Script.Clone();
            ApplyChanges(script);
            var task = string.IsNullOrEmpty(script.ETag)
                ? CreateScriptAsync(script)
                : ReplaceScriptAsync(script);
            await task;
            Script = script;
            RefreshCommands();
        }

        protected virtual void ApplyChanges(TScript script)
        {
            script.Body = Text;
        }

        protected abstract Task CreateScriptAsync(TScript script);
        protected abstract Task ReplaceScriptAsync(TScript script);

        private bool CanSave() => HasChanges;

        private DelegateCommand? _revertCommand;
        public ICommand RevertCommand => _revertCommand ??= new DelegateCommand(Revert, CanRevert);

        protected virtual void Revert()
        {
            Text = Script.Body;
        }

        private bool CanRevert() => HasChanges;

        public override bool HasChanges => Text != Script.Body;

        protected void RefreshCommands()
        {
            _saveCommand?.RaiseCanExecuteChanged();
            _revertCommand?.RaiseCanExecuteChanged();
        }
    }
}
