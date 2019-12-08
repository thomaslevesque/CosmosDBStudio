using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services.Implementation
{
    public abstract class RequestOptionsBuilderBase<TBuilder, TOptions>
        : IRequestOptionsBuilder<TBuilder, TOptions>
        where TBuilder : IRequestOptionsBuilder<TBuilder, TOptions>
        where TOptions : RequestOptions, new()
    {
        protected abstract void Build(TOptions options);

        public TOptions Build()
        {
            var options = new TOptions();
            Build(options);
            return options;
        }
    }
}
