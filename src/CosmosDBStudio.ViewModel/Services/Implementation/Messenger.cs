using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.Util.Extensions;

namespace CosmosDBStudio.ViewModel.Services.Implementation
{
    public class Messenger : IMessenger
    {
        private readonly LinkedList<IWeakHandler> _weakHandlers = new LinkedList<IWeakHandler>();

        public void Publish<TMessage>(TMessage message)
        {
            List<LinkedListNode<IWeakHandler>>? deadNodes = null;

            var nodes = GetNodes();

            foreach (var node in nodes)
            {
                var weakHandler = node.Value;
                if (!weakHandler.IsAlive)
                {
                    AddDeadNode(node);
                }
                else if (weakHandler.CanHandle(typeof(TMessage)) && !weakHandler.TryInvoke(message!))
                {
                    AddDeadNode(node);
                }
            }

            PruneDeadNodes();

            IList<LinkedListNode<IWeakHandler>> GetNodes()
            {
                lock (_weakHandlers)
                {
                    return _weakHandlers.Nodes().ToList();
                }
            }

            void AddDeadNode(LinkedListNode<IWeakHandler> node)
            {
                deadNodes ??= new List<LinkedListNode<IWeakHandler>>();
                deadNodes.Add(node);
            }

            void PruneDeadNodes()
            {
                if (deadNodes is null)
                    return;

                lock (_weakHandlers)
                {
                    foreach (var node in deadNodes)
                    {
                        if (node.List == _weakHandlers)
                            _weakHandlers.Remove(node);
                    }
                }
            }
        }

        public IMessengerSubscriptionBuilder<TSubscriber> Subscribe<TSubscriber>(TSubscriber subscriber)
            where TSubscriber : class
        {
            return new MessengerSubscriptionBuilder<TSubscriber>(this, subscriber);
        }

        private IDisposable AddHandler<TSubscriber, TMessage>(TSubscriber subscriber, Action<TSubscriber, TMessage> handler, bool handleDerivedTypes)
            where TSubscriber : class
        {
            var weakHandler = new WeakHandler<TSubscriber, TMessage>(
                subscriber,
                handler,
                handleDerivedTypes);

            lock (_weakHandlers)
            {
                _weakHandlers.AddLast(weakHandler);
            }

            return weakHandler;
        }

        private interface IWeakHandler
        {
            bool IsAlive { get; }
            bool CanHandle(Type messageType);
            bool TryInvoke(object message);
        }

        private class WeakHandler<TSubscriber, TMessage> : IWeakHandler, IDisposable
            where TSubscriber : class
        {
            private readonly WeakReference<TSubscriber> _subscriber;
            private readonly Action<TSubscriber, TMessage> _handler;
            private readonly bool _handleDerivedTypes;

            public WeakHandler(TSubscriber subscriber, Action<TSubscriber, TMessage> handler, bool handleDerivedTypes)
            {
                _subscriber = new WeakReference<TSubscriber>(subscriber);
                _handler = handler;
                _handleDerivedTypes = handleDerivedTypes;
            }

            public bool IsAlive => _subscriber.TryGetTarget(out _);

            public bool CanHandle(Type messageType)
            {
                if (messageType == typeof(TMessage))
                    return true;
                return _handleDerivedTypes && typeof(TMessage).IsAssignableFrom(messageType);
            }

            public bool TryInvoke(object message)
            {
                if (_subscriber.TryGetTarget(out var subscriber))
                {
                    _handler(subscriber, (TMessage)message);
                    return true;
                }

                return false;
            }

            public void Dispose()
            {
                _subscriber.SetTarget(null!);
            }
        }

        private class MessengerSubscriptionBuilder<TSubscriber> : IMessengerSubscriptionBuilder<TSubscriber>
            where TSubscriber : class
        {
            private readonly Messenger _messenger;
            private readonly TSubscriber _subscriber;

            public MessengerSubscriptionBuilder(Messenger messenger, TSubscriber subscriber)
            {
                _messenger = messenger;
                _subscriber = subscriber;
            }

            public IDisposable To<TMessage>(Action<TSubscriber, TMessage> handler, bool handleDerivedTypes = false)
            {
                return _messenger.AddHandler(_subscriber, handler, handleDerivedTypes);
            }
        }
    }
}