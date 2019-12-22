using System;

namespace CosmosDBStudio.Services
{
    public interface IUIDispatcher
    {
        void Invoke(Action action);
    }
}
