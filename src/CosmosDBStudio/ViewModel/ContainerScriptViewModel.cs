using CosmosDBStudio.Model;

namespace CosmosDBStudio.ViewModel
{
    public abstract class ContainerScriptViewModel : TreeNodeViewModel
    {
    }

    public class ContainerScriptViewModel<TScript> : ContainerScriptViewModel
        where TScript : ICosmosItem
    {
        public ContainerScriptViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            TScript script)
        {
            Container = container;
            Parent = parent;
            Script = script;
        }

        protected TScript Script { get; }

        public override string Text => Script.Id;
        public override NonLeafTreeNodeViewModel? Parent { get; }
        public ContainerViewModel Container { get; }
    }
}
