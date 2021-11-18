using System;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel.Services
{
    public interface IUIDispatcher
    {
        void Invoke(Action action);
        Task InvokeAsync(Action action);
    }
}
