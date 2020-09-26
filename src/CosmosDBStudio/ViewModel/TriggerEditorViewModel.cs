using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using Microsoft.Azure.Cosmos.Scripts;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class TriggerEditorViewModel : ScriptEditorViewModelBase<CosmosTrigger>
    {
        public TriggerEditorViewModel(CosmosTrigger trigger, IContainerContext containerContext)
            : base(trigger, containerContext)
        {
            Type = trigger.Type;
            Operation = trigger.Operation;
        }

        public override string Description => "trigger";

        protected override void ApplyChanges(CosmosTrigger script)
        {
            base.ApplyChanges(script);
            script.Type = Type;
            script.Operation = Operation;
        }

        protected override void Revert()
        {
            base.Revert();
            Type = Script.Type;
            Operation = Script.Operation;
        }

        protected override Task CreateScriptAsync(CosmosTrigger script) =>
            ContainerContext.Scripts.CreateTriggerAsync(script, default);

        protected override Task ReplaceScriptAsync(CosmosTrigger script) =>
            ContainerContext.Scripts.ReplaceTriggerAsync(script, default);

        private TriggerOperation _operation;
        public TriggerOperation Operation
        {
            get => _operation;
            set => Set(ref _operation, value)
                .AndNotifyPropertyChanged(nameof(HasChanges))
                .AndExecute(RefreshCommands);
        }

        private TriggerType _type;
        public TriggerType Type
        {
            get => _type;
            set => Set(ref _type, value)
                .AndNotifyPropertyChanged(nameof(HasChanges))
                .AndExecute(RefreshCommands);
        }

        public override bool HasChanges => base.HasChanges
            || Type != Script.Type
            || Operation != Script.Operation;
    }
}
