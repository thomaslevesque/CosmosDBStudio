namespace CosmosDBStudio.Model;

/// <summary>
///     Interface for a generic tree node that can be displayed
///     on the tree.
/// </summary>
public interface ITreeNode
{
    /// <summary>
    ///     The name to be displayed on the tree for the tree node
    /// </summary>
    public string DisplayName { get; }
}