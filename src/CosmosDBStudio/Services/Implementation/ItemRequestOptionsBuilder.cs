using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services.Implementation
{
    public class ItemRequestOptionsBuilder
        : RequestOptionsBuilderBase<IItemRequestOptionsBuilder, ItemRequestOptions>,
          IItemRequestOptionsBuilder
    {
        private string? _ifMatchEtag;

        public IItemRequestOptionsBuilder IfMatch(string? eTag)
        {
            _ifMatchEtag = eTag;
            return this;
        }

        protected override void Build(ItemRequestOptions options)
        {
            options.IfMatchEtag = _ifMatchEtag;
        }
    }
}
