using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public abstract class ScriptNodeViewModel : TreeNodeViewModel
    {
    }

    public abstract class ScriptNodeViewModel<TScript> : ScriptNodeViewModel
        where TScript : ICosmosScript, new()
    {
        private readonly ScriptCommands<TScript> _commands;

        protected ScriptNodeViewModel(
            TScript script,
            IContainerContext context,
            NonLeafTreeNodeViewModel parent,
            ScriptCommands<TScript> commands,
            IMessenger messenger)
        {
            Parent = parent;
            Script = script;
            Context = context;
            _commands = commands;
            Messenger = messenger;
            Commands = new[]
            {
                new CommandViewModel($"Open {Description}", commands.OpenCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel($"Create new {Description}", commands.CreateCommand, parent),
                new CommandViewModel($"Delete {Description}", commands.DeleteCommand, this),
            };
        }

        public TScript Script { get; }
        public IContainerContext Context { get; }
        public IMessenger Messenger { get; }

        public override string Text => Script.Id;
        public override NonLeafTreeNodeViewModel? Parent { get; }

        public override IEnumerable<CommandViewModel> Commands { get; }

        public abstract string Description { get; }

        private DelegateCommand? _openCommand;
        public ICommand OpenCommand => _openCommand ??= _commands.OpenCommand.WithParameter(this);

        public abstract Task DeleteAsync();
    }
}
