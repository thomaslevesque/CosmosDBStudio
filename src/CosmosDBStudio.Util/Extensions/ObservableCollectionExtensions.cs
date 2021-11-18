using System.Collections.ObjectModel;

namespace CosmosDBStudio.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void PushMRU<T>(this ObservableCollection<T> mruList, T value, int maxMRUCount)
        {
            int index = mruList.IndexOf(value);
            if (index == 0)
            {
                return;
            }
            else if (index > 0)
            {
                mruList.Move(index, 0);
            }
            else
            {
                mruList.Insert(0, value);
            }

            while (mruList.Count > maxMRUCount)
            {
                mruList.RemoveAt(mruList.Count - 1);
            }
        }
    }
}
