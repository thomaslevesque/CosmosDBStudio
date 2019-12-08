using Microsoft.Azure.Cosmos;
using System;

namespace CosmosDBStudio.Services.Implementation
{
    public static class PartitionKeyHelper
    {
        public static PartitionKey? Create(object? value)
        {
            return value switch
            {
                null => default(PartitionKey?),
                string s => new PartitionKey(s),
                double d => new PartitionKey(d),
                bool b => new PartitionKey(b),
                _ => throw new ArgumentException("Invalid partition key type")
            };
        }
    }
}
