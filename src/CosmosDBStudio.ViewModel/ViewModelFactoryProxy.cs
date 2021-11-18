using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace CosmosDBStudio.ViewModel
{
    public static class ViewModelFactoryProxy
    {
        public static IViewModelFactory Create(IServiceProvider sp)
        {
            var generator = new ProxyGenerator();
            return generator.CreateInterfaceProxyWithoutTarget<IViewModelFactory>(
                new ViewModelFactoryInterceptor(sp));
        }

        private class ViewModelFactoryInterceptor : IInterceptor
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly ConcurrentDictionary<MethodInfo, ObjectFactory> _factories;

            public ViewModelFactoryInterceptor(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
                _factories = new ConcurrentDictionary<MethodInfo, ObjectFactory>();
            }

            public void Intercept(IInvocation invocation)
            {
                var factory = _factories.GetOrAdd(invocation.Method, CreateFactory);
                invocation.ReturnValue = factory(_serviceProvider, invocation.Arguments);
            }

            private ObjectFactory CreateFactory(MethodInfo method)
            {
                return ActivatorUtilities.CreateFactory(
                    method.ReturnType,
                    method.GetParameters().Select(p => p.ParameterType).ToArray());
            }
        }
    }
}
