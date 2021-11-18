using CosmosDBStudio.Model.Services;

namespace CosmosDBStudio.ViewModel.Messages
{
    public class NewQuerySheetMessage
    {
        public NewQuerySheetMessage(IContainerContext context)
        {
            Context = context;
        }

        public IContainerContext Context { get; }
    }
}
