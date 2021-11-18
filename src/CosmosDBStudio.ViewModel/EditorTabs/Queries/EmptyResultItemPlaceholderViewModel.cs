namespace CosmosDBStudio.ViewModel.EditorTabs.Queries
{
    public class EmptyResultItemPlaceholderViewModel : ResultItemViewModel
    {
        public override string DisplayValue => "(no results)";

        public override object? PartitionKey => null;

        public override string Text
        {
            get => DisplayValue;
            set { }
        }

        public override bool IsJson => false;
    }
}
