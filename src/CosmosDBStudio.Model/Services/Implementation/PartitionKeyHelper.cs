using System;
using Hamlet;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Model.Services.Implementation
{
    public static class PartitionKeyHelper
    {
        public static PartitionKey? Create(Option<object?> partitionKey)
        {
            if (partitionKey.TryGetValue(out object? value))
                return Create(value);

            return null;
        }

        public static PartitionKey Create(object? partitionKey)
        {
            return partitionKey switch
            {
                null => PartitionKey.Null,
                string s => new PartitionKey(s),
                double d => new PartitionKey(d),
                bool b => new PartitionKey(b),
                _ => throw new ArgumentException("Invalid partition key type")
            };
        }
    }
}
