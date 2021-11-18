using CosmosDBStudio.Services;

namespace CosmosDBStudio.Messages
{
    public class OpenScriptMessage<TScript>
    {
        public OpenScriptMessage(IContainerContext context, TScript script)
        {
            Context = context;
            Script = script;
        }

        public IContainerContext Context { get; }
        public TScript Script { get; }
    }
}
