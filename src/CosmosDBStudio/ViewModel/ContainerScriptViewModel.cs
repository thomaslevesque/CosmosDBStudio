using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public abstract class ContainerScriptViewModel : TreeNodeViewModel
    {
    }

    public class ContainerScriptViewModel<TScript> : ContainerScriptViewModel
        where TScript : ICosmosScript
    {
        public ContainerScriptViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            TScript script,
            IMessenger messenger)
        {
            Container = container;
            Parent = parent;
            Script = script;
            Messenger = messenger;
        }

        public TScript Script { get; }
        public IMessenger Messenger { get; }

        public override string Text => Script.Id;
        public override NonLeafTreeNodeViewModel? Parent { get; }
        public ContainerViewModel Container { get; }

        private DelegateCommand? _openCommand;
        public ICommand OpenCommand => _openCommand ??= new DelegateCommand(Edit);

        private void Edit()
        {
            Messenger.Publish(new OpenScriptMessage<TScript>(
                Container.Database.Account.Id,
                Container.Database.Id,
                Container.Id,
                Script));
        }
    }
}
