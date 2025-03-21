using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SugarNode
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private List<Kvp> serializableList;
        public void OnAfterDeserialize()
        {
            foreach (var listItem in serializableList)
                TryAdd(listItem.key, listItem.value);
            serializableList.Clear();
            serializableList = null;
        }
        public void OnBeforeSerialize()
        {
            serializableList = this.Select(kvp => new Kvp
            {
                key = kvp.Key,
                value = kvp.Value
            }).ToList();
        }
        [HideInInspector, Serializable]
        private class Kvp
        {
            public TKey key;
            public TValue value;
        }
    }
    [Serializable, HideInInspector]
    public sealed class ConnectionNets : SerializableDictionary<string, HashSet<string>>, ISerializationCallbackReceiver
    {
        public void AddConnectGUID(string input, string output)
        {
            if (!TryGetValue(output, out var inputs))
                Add(output, new HashSet<string>() { input });
            else inputs.Add(input);
        }
        public void RemoveConnectGUID(string input, string output)
        {
            if (TryGetValue(output, out var inputs))
            {
                inputs.Remove(input);
                if (inputs.Count == 0)
                    Remove(output);
            }
        }
    }
}