﻿using Hamlet;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Model.Services.Implementation
{
    public class QueryRequestOptionsBuilder
        : RequestOptionsBuilderBase<IQueryRequestOptionsBuilder, QueryRequestOptions>,
          IQueryRequestOptionsBuilder
    {
        private Option<object?> _partitionKey;
        private int? _maxItemCount;

        public IQueryRequestOptionsBuilder WithPartitionKey(Option<object?> partitionKey)
        {
            _partitionKey = partitionKey;
            return this;
        }

        public IQueryRequestOptionsBuilder WithMaxItemCount(int? maxItemCount)
        {
            _maxItemCount = maxItemCount;
            return this;
        }

        protected override void Build(QueryRequestOptions options)
        {
            options.PartitionKey = PartitionKeyHelper.Create(_partitionKey);
            options.MaxItemCount = _maxItemCount;
        }
    }
}
