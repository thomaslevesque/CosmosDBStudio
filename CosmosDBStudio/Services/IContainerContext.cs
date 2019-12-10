namespace CosmosDBStudio.Services
{
    public interface IContainerContext
    {
        string AccountId { get; }
        string AccountName { get; }
        string DatabaseId { get; }
        string ContainerId { get; }
        string? PartitionKeyPath { get; }
        string? PartitionKeyJsonPath { get; }
        IDocumentService Documents { get; }
        IQueryService Query { get; }
    }
}
