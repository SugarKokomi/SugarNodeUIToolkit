using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace SugarNode
{
    public abstract class Graph : ScriptableObject
    {
        private List<Node> nodes = new List<Node>();
        public IEnumerable<Node> Nodes => nodes;
#if UNITY_EDITOR
        // internal event Action onNodesChange;
        public void AddNode(Node node)
        {
            nodes.Add(node);
            node.m_graph = this;
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
        }
        public bool DeleteNode(Node node)
        {
            if (nodes.Remove(node))
            {
                AssetDatabase.RemoveObjectFromAsset(node);
                AssetDatabase.SaveAssets();
                return true;
            }
            return false;
        }
#endif
    }
}