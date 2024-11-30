using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SugarNode
{
    [HideInInspector, Serializable]
    public abstract class Port : ICanClearConnectionCache
    //Port无法自己建立缓存，其连接网络需要Graph拿到全部Node才能帮忙构建。但是它可以自己清除自己的缓存
    {
        internal Port(Func<List<Port>, List<Port>> connectionCondition = null)
        {
            WhoCanConnectMe = connectionCondition ?? DefaultConnectCondition;
            GetValue = DefaultGetValue;
        }

        //自己的GUID，用于数据持久化存储
        [SerializeField, HideInInspector]
        internal string m_guid;
        /// <summary> 用于Port的唯一标识 </summary>
        public string guid => m_guid;

        [SerializeField, HideInInspector]
        internal int m_maxConnectionCount = -1;
        /// <summary> 端口的最大连接数量。 </summary>
        public int maxConnectionCount => m_maxConnectionCount;

        /// <summary> 端口所归属的节点 </summary>
        [SerializeField, HideInInspector]
        internal Node m_node;
        public Node Node => m_node;

        public Func<List<Port>, List<Port>> WhoCanConnectMe;
        public virtual List<Port> DefaultConnectCondition(List<Port> allPorts)
        {
            return allPorts.Where(otherPort =>
                otherPort.m_node != this.m_node//不能自己节点连自己节点
            ).ToList();
        }
        public Func<object> GetValue;
        protected virtual object DefaultGetValue() => null;
        public abstract void DisposeCache();//以抽象方法要求每个子类实现
    }
    [HideInInspector, Serializable]
    public class InputPort : Port
    {
        /// <summary> 用于运行时和开发时构建，快速存取读取 </summary>
        [NonSerialized]
        internal HashSet<OutputPort> m_connections = new HashSet<OutputPort>();
        public IEnumerable<OutputPort> Connections => m_connections;

        public override void DisposeCache()
        {
            m_connections.Clear();
        }
    }
    [HideInInspector, Serializable]
    public class OutputPort : Port
    {
        /// <summary> 用于运行时和开发时构建，快速存取读取 </summary>
        [NonSerialized]
        internal HashSet<InputPort> m_connections = new HashSet<InputPort>();
        public IEnumerable<InputPort> Connections => m_connections;
        /// <summary> 用于数据存取 序列化 </summary>
        [SerializeField, HideInInspector]
        internal List<string> m_connectionsGUID = new List<string>();
        public IEnumerable<string> connectionsGUID => m_connectionsGUID;

        public override void DisposeCache()
        {
            m_connections.Clear();
        }
    }
    [HideInInspector, Serializable]
    public class InputPort<T> : InputPort
    {
        public T Value;
    }
    [HideInInspector, Serializable]
    public class OutputPort<T> : OutputPort
    {
        public T Value;
    }
}