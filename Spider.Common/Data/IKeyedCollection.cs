using System.Collections.Generic;

namespace Spider.Data
{
    /// <summary>
    /// 有键的集合
    /// </summary>
    /// <typeparam name="TKey">键</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    public interface IKeyedCollection<TKey, TValue> : ICollection<TValue>
    {
        /// <summary>
        /// 按照索引检索元素
        /// </summary>
        /// <param name="index">要检索的元素的索引</param>
        /// <returns>检索的元素</returns>
        TValue this[int index] { get; }

        /// <summary>
        /// 按照名称检索元素
        /// </summary>
        /// <param name="key">要检索的元素的键</param>
        /// <returns>检索的元素，如果没有找到将抛出异常</returns>
        TValue this[TKey key] { get; }

        /// <summary>
        /// 确定 集合 是否包含指定的键。
        /// </summary>
        /// <param name="key">要在 集合 中定位的键。</param>
        /// <returns>如果 集合 包含具有指定键的元素，则为 true；否则为 false。</returns>
        bool Contains(TKey key);

        /// <summary>
        /// 从集合中尝试使用指定的名称检索元素
        /// </summary>
        /// <param name="key">尝试检索的键</param>
        /// <param name="item">如果寻找到此元素，将赋值此元素，否则为default(T)</param>
        /// <returns>如果寻找到此元素，将返回true，否则返回false</returns>
        bool TryGet(TKey key, out TValue item);

        /// <summary>
        /// 确定 INamedCollection`1 中特定项的索引。
        /// </summary>
        /// <param name="item">IKeyedCollection&lt;TKey,TValue&gt; 中定位的对象。</param>
        /// <returns>如果在列表中找到，则为 item 的索引；否则为 -1。</returns>
        int IndexOf(TValue item);

        /// <summary>
        /// 移除指定索引处的IKeyedCollection&lt;TKey,TValue&gt; 项。
        /// </summary>
        /// <param name="index">从零开始的索引（属于要移除的项）。</param>
        void RemoveAt(int index);

        /// <summary>
        /// 从 IKeyedCollection&lt;TKey,TValue&gt; 中移除带有指定键的元素。
        /// </summary>
        /// <param name="key">要移除的元素的键。</param>
        /// <returns>如果该元素已成功移除，则为 true；否则为 false。
        /// 如果在 IKeyedCollection&lt;TKey,TValue&gt; 中没有找到 key，此方法也会返回 false。</returns>
        bool Remove(TKey key);
    }

    public interface INamedCollection<T> : IKeyedCollection<string, T> where T : INamedObject
    {
    }
}

