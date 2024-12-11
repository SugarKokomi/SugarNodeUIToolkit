using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SugarNode
{
    [HideInInspector, Serializable]
    public abstract class Port
    //Port无法自己建立缓存，其连接网络需要Graph拿到全部Node才能帮忙构建。但是它可以自己清除自己的缓存
    {
        internal Port(string portName, Func<List<Port>, List<Port>> connectionCondition)
        {
            this.portName = portName;
            WhoCanConnectMe = connectionCondition ?? DefaultConnectCondition;
            getValue = GetValue;
        }

        //自己的GUID，用于数据持久化存储
        [SerializeField, HideInInspector]
        internal string m_guid;

        [SerializeField, HideInInspector]
        internal string portName;
        public string PortName => portName;
        /// <summary> 用于Port的唯一标识 </summary>
        public string guid => m_guid;

        [SerializeField, HideInInspector]
        internal int m_maxConnectionCount = -1;
        /// <summary> 端口的最大连接数量。 </summary>
        public int maxConnectionCount => m_maxConnectionCount;

        /// <summary> 端口所归属的节点 </summary>
        internal Node m_node;
        public Node Node => m_node;
        [NonSerialized]
        protected bool hadInitSelf = false;
        public Func<List<Port>, List<Port>> WhoCanConnectMe;
        public virtual List<Port> DefaultConnectCondition(List<Port> allPorts)
        {
            return allPorts.Where(otherPort =>
                otherPort.m_node != this.m_node//不能自己节点连自己节点
            ).ToList();
        }
        public Func<object> getValue;
        public virtual object GetValue() => null;
    }
    [HideInInspector, Serializable]
    public class InputPort : Port, ICanBulidConnectionCache
    {
        /// <summary> 用于运行时和开发时构建，快速存取读取 </summary>
        [NonSerialized]
        internal HashSet<OutputPort> m_connections = new HashSet<OutputPort>();
        public IEnumerable<OutputPort> Connections => m_connections;
        public OutputPort Connection => m_connections.FirstOrDefault();
        public InputPort(string portName = "input", Func<List<Port>, List<Port>> connectionCondition = null) :
            base(portName, connectionCondition)
        { }
        void ICanBulidConnectionCache.InitCache()
        {
            if (hadInitSelf) return;
            hadInitSelf = true;
            m_connections = new HashSet<OutputPort>();
        }
        void ICanClearConnectionCache.DisposeCache()
        {
            if (!hadInitSelf) return;
            hadInitSelf = false;
            m_node = null;
            m_connections.Clear();
            m_connections = null;
        }
        public override object GetValue()
        {
            //InputPort取值规则：若自身无连接，使用默认值。若有连接，问OutputPort要值
            if (m_connections.Count == 0)
                return null;
            else
                return Connection.getValue();
        }
    }
    [HideInInspector, Serializable]
    public class OutputPort : Port, ICanBulidConnectionCache
    {
        /// <summary> 用于运行时和开发时构建，快速存取读取 </summary>
        [NonSerialized]
        internal HashSet<InputPort> m_connections = new HashSet<InputPort>();
        public IEnumerable<InputPort> Connections => m_connections;
        public InputPort Connection => m_connections.FirstOrDefault();
        /// <summary> 用于数据存取 序列化 </summary>
        [SerializeField, HideInInspector]
        internal List<string> m_connectionsGUID = new List<string>();
        public IEnumerable<string> connectionsGUID => m_connectionsGUID;

        public OutputPort(string portName = "output", Func<List<Port>, List<Port>> connectionCondition = null) :
            base(portName, connectionCondition)
        {
        }
        void ICanBulidConnectionCache.InitCache()
        {
            if (hadInitSelf) return;
            hadInitSelf = true;
            m_connections = new HashSet<InputPort>();
        }
        void ICanClearConnectionCache.DisposeCache()
        {
            if (!hadInitSelf) return;
            hadInitSelf = false;
            m_node = null;
            m_connections.Clear();
            m_connections = null;
        }
        public override object GetValue()
        {
            //OutputPort取值规则：若自身无连接，使用默认值。若有连接，问Node要值
            if (m_connections.Count == 0)
                return null;
            else
                return Node.GetValue(this);
        }
    }
    [HideInInspector, Serializable]
    public class InputPort<T> : InputPort
    {
        public T defaultValue;

        public InputPort(string portName = "input", T value = default, Func<List<Port>, List<Port>> connectionCondition = null) :
            base(portName, connectionCondition)
        {
            defaultValue = value;
        }
        public override object GetValue()
        {
            //InputPort取值规则：若自身无连接，使用默认值。若有连接，问OutputPort要值
            if (m_connections.Count == 0)
                return defaultValue;
            else
                return Connection.getValue();
        }
    }
    [HideInInspector, Serializable]
    public class OutputPort<T> : OutputPort
    {
        public T defaultValue;
        public OutputPort(string portName = "output", T value = default, Func<List<Port>, List<Port>> connectionCondition = null) :
            base(portName, connectionCondition)
        {
            defaultValue = value;
        }
        public override object GetValue()
        {
            //OutputPort取值规则：若自身无连接，使用默认值。若有连接，问Node要值
            if (m_connections.Count == 0)
                return defaultValue;
            else
                return Node.GetValue(this);
        }
    }
}