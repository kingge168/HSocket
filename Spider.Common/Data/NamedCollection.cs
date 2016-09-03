using System;
using System.Collections.ObjectModel;

namespace Spider.Data
{
    [Serializable]
    public class NamedCollection<T> : KeyedCollection<string, T>, INamedCollection<T> where T : INamedObject
    {
        protected override string GetKeyForItem(T item)
        {
            return item.Name;
        }

        public bool TryGet(string key, out T item)
        {
            if (key == null)
            {
                item = default(T);
                return false;
            }
            if (this.Dictionary == null)
            {
                foreach (T item2 in base.Items)
                {
                    if (Comparer.Equals(key, this.GetKeyForItem(item2)))
                    {
                        item = item2;
                        return true;
                    }
                }
                item = default(T);
                return false;
            }
            return this.Dictionary.TryGetValue(key, out item);
        }
    }
}

