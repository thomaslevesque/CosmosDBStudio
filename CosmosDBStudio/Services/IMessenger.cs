using System;

namespace CosmosDBStudio.Services
{
    public interface IMessenger
    {
        void Publish<TMessage>(TMessage message);

        IMessengerSubscriptionBuilder<TSubscriber> Subscribe<TSubscriber>(TSubscriber subscriber)
            where TSubscriber : class;
    }

    public interface IMessengerSubscriptionBuilder<out TSubscriber>
        where TSubscriber : class
    {
        IDisposable To<TMessage>(Action<TSubscriber, TMessage> handler, bool handleDerivedTypes = false);
    }
}
