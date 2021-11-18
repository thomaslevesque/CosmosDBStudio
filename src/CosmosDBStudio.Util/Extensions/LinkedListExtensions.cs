using System.Collections.Generic;

namespace CosmosDBStudio.Util.Extensions
{
    public static class LinkedListExtensions
    {
        public static IEnumerable<LinkedListNode<T>> Nodes<T>(this LinkedList<T> linkedList)
        {
            var node = linkedList.First;
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }
    }
}
