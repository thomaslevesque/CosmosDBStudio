using CosmosDBStudio.Services;

namespace CosmosDBStudio.Messages
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
