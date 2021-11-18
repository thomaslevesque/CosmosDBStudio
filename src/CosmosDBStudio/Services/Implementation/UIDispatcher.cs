using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using CosmosDBStudio.ViewModel.Services;

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

        public Task InvokeAsync(Action action)
        {
            var operation = _dispatcher.Value.InvokeAsync(action);
            return operation.Task;
        }
    }
}
