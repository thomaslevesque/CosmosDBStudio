namespace CosmosDBStudio.ViewModel
{
    public class ErrorItemPlaceholderViewModel : ResultItemViewModel
    {
        public override string DisplayId => "(error)";

        public override string PartitionKey => null;

        public override string Text
        {
            get => "(error)";
            set { }
        }

        public override bool IsJson => false;

        public override bool IsReadOnly => true;
    }
}
