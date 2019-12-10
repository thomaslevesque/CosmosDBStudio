namespace CosmosDBStudio.ViewModel
{
    public class EmptyResultItemPlaceholderViewModel : ResultItemViewModel
    {
        public override string DisplayId => "(no results)";

        public override object? PartitionKey => null;

        public override string Text
        {
            get => DisplayId;
            set { }
        }

        public override bool IsJson => false;

        public override bool IsReadOnly => true;
    }
}
