using System.Linq;

namespace CosmosDBStudio.Model.Helpers
{
    public static class CosmosHelper
    {
        private static readonly char[] ForbiddenCharactersInId = @"/\#?".ToCharArray();

        public static string ValidateId(string id, string? idNameInMessage = null)
        {
            idNameInMessage ??= "id";

            if (string.IsNullOrEmpty(id))
                return $"The {idNameInMessage} must be specified";

            if (id.IndexOfAny(ForbiddenCharactersInId) >= 0)
            {
                return $"The {idNameInMessage} must not contain characters "
                    + string.Join(" ", ForbiddenCharactersInId.Select(c => $"'{c}'"));
            }

            if (id.Trim() != id)
            {
                return $"The {idNameInMessage} must not start or end with space";
            }

            return string.Empty;
        }

        public static string? ValidateThroughput(int? throughput) => ValidateThroughput(throughput ?? 0, throughput.HasValue);

        public static string ValidateThroughput(int throughput, bool isThroughputProvisioned)
        {
            if (isThroughputProvisioned)
            {
                if (throughput < 400)
                    return "Throughput cannot be less than 400";
            }

            return string.Empty;
        }

        public static string ValidateDefaultTTL(int? defaultTTL) => ValidateDefaultTTL(defaultTTL ?? 0, defaultTTL.HasValue);

        public static string ValidateDefaultTTL(int defaultTTL, bool hasDefaultTTL)
        {
            if (hasDefaultTTL)
            {
                if (defaultTTL < 1)
                    return "Default TTL must be at least 1 second";
            }

            return string.Empty;
        }

        public static string ValidatePartitionKeyPath(string? partitionKeyPath)
        {
            if (string.IsNullOrEmpty(partitionKeyPath))
                return "The partition key must be specified";

            if (partitionKeyPath.Trim() != partitionKeyPath)
                return "The partition key must not start or end with space";

            if (!partitionKeyPath.StartsWith('/'))
                return "The partition key must start with '/'";

            return string.Empty;
        }
    }
}
