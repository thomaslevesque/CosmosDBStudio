namespace CosmosDBStudio.ViewModel.EditorTabs.Queries
{
    public class ErrorItemPlaceholderViewModel : ResultItemViewModel
    {
        public override string DisplayValue => "(error)";

        public override object? PartitionKey => null;

        public override string Text
        {
            get => "(error)";
            set { }
        }

        public override bool IsJson => false;
    }
}
