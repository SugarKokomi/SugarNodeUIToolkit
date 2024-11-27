#define SugarNode
using UnityEngine;
using System.Collections.Generic;

namespace SugarNode
{
    public abstract class Node : ScriptableObject
    {
        internal Graph m_graph;
        public Graph graph => m_graph;
        [HideInInspector]
        public string guid;
        // protected internal abstract HashSet<NodePort> InitInputPort();
        // protected internal abstract HashSet<NodePort> InitOutputPort();
#if UNITY_EDITOR
        [HideInInspector]
        public Vector2 gridPos = Vector2.zero;
#endif
    }
    public sealed class Empty { }
    public sealed class NodePort : NodePort<Empty> { }
    public class NodePort<T>
    {
        internal HashSet<NodePort<T>> connection = new HashSet<NodePort<T>>();
        public Node Node { get; internal set; }
        public bool ConnectTo(NodePort<T> port)
        {
            if (connection.Contains(port))
                return false;
            else
            {
                connection.Add(port);
                return true;
            }
        }
        public bool DisConnectTo(NodePort<T> port)
        {
            if (!connection.Contains(port))
                return false;
            else
            {
                connection.Remove(port);
                return true;
            }
        }
        public bool IsConnectTo(NodePort<T> port)
        {
            return connection.Contains(port);
        }
    }
    public enum PortDirection { Horizontal, Vertical }
    public enum PortCapacity { Single, Multi }
}