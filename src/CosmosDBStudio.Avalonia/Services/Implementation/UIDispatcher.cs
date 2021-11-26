using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CosmosDBStudio.ViewModel.Services;

namespace CosmosDBStudio.Avalonia.Services.Implementation;

public class UIDispatcher : IUIDispatcher
{
    public void Invoke(Action action) => Dispatcher.UIThread.Post(action);

    public Task InvokeAsync(Action action) => Dispatcher.UIThread.InvokeAsync(action);
}