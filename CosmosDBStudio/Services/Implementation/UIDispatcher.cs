using System;
using System.Windows.Threading;

namespace CosmosDBStudio.Services.Implementation
{
    public class UIDispatcher : IUIDispatcher
    {
        private readonly Lazy<Dispatcher> _dispatcher;

        public UIDispatcher(Lazy<Dispatcher> dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            _dispatcher.Value.Invoke(action);
        }
    }
}
