using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public abstract class ContainerScriptViewModel : TreeNodeViewModel
    {
    }

    public abstract class ContainerScriptViewModel<TScript> : ContainerScriptViewModel
        where TScript : ICosmosScript, new()
    {
        private readonly ScriptCommands<TScript> _commands;

        protected ContainerScriptViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            TScript script,
            ScriptCommands<TScript> commands,
            IMessenger messenger)
        {
            Container = container;
            Parent = parent;
            Script = script;
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
        public IMessenger Messenger { get; }

        public override string Text => Script.Id;
        public override NonLeafTreeNodeViewModel? Parent { get; }
        public ContainerViewModel Container { get; }

        public override IEnumerable<CommandViewModel> Commands { get; }

        public abstract string Description { get; }

        private DelegateCommand? _openCommand;
        public ICommand OpenCommand => _openCommand ??= _commands.OpenCommand.WithParameter(this);

        public abstract Task DeleteAsync(ICosmosAccountManager accountManager);
    }
}
