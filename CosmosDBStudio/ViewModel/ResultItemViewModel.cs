using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public abstract class ResultItemViewModel : BindableBase
    {
        public abstract string DisplayId { get; }
        public abstract object? PartitionKey { get; }
        public abstract string Text { get; set; }
        public abstract bool IsJson { get; }
    }
}
