using System.Collections.Generic;

namespace SugarNode
{
    internal static class DictionaryExtension
    {
        /// <summary> 
        /// 为字典添加或替换一个键值对。
        /// <para>在较低版本.Net中，无法直接使用语法糖进行添加=> dic[key]=value </para>
        /// </summary>
        internal static bool AddOrSetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            var isContains = dictionary.ContainsKey(key);
            if (isContains) dictionary[key] = value;
            else dictionary.Add(key, value);
            return isContains;
        }
    }
    /// <summary> 可以清除自己的连接缓存 </summary>
    internal interface ICanClearConnectionCache
    {
        /// <summary> 定义释放自身缓存的方法 </summary>
        void DisposeCache();
    }
    /// <summary> 可以构建自己的连接缓存 </summary>
    internal interface ICanBulidConnectionCache : ICanClearConnectionCache
    {
        /// <summary> 定义构建自己缓存的方法 </summary>
        void InitCache();
    }
}