using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SugarNode
{

    [HideInInspector, Serializable]
    public abstract class Port : ICanBulidConnectionCache
    //Port无法自己建立缓存，其连接网络需要Graph拿到全部Node才能帮忙构建。但是它可以自己清除自己的缓存
    {
        public enum PortDirection { Input, Output }
        protected Port(string portName)
        {
            this.portName = portName;
            CanConnected = DefaultConnectCondition;
            if (string.IsNullOrEmpty(m_guid))//若guid为null，说明该Port是new出来的。若不为null，说明是Unity序列化出来的
            {
                m_guid = UnityEditor.GUID.Generate().ToString();
            }
        }
        protected abstract PortDirection Direction { get; }

        [SerializeField, HideInInspector]
        internal string m_guid;
        /// <summary> 用于Port的唯一标识 </summary>
        public string guid => m_guid;

        [SerializeField, HideInInspector]
        internal string portName;
        /// <summary> 端口名。可用于简易备注，或者查找。也可起到类似Odin[LabelText]的作用 </summary>
        public string PortName => portName;


        [SerializeField, HideInInspector]
        internal int m_maxConnectionCount = -1;
        /// <summary> 端口的最大连接数量。 </summary>
        public int maxConnectionCount => m_maxConnectionCount;


        /// <summary> 端口所归属的节点 </summary>
        [SerializeField, HideInInspector]
        internal Node m_node;
        public Node Node => m_node;

        protected bool hadInitSelf = false;
        public Func<Port, bool> CanConnected;
        public bool DefaultConnectCondition(Port other)
        {
            return other.Node != this.Node && other.Direction != this.Direction;
        }
        public virtual object GetValue() => null;

        public void InitCache()
        {

        }

        public void DisposeCache()
        {

        }
    }
    [/* HideInInspector, */ Serializable]
    public class InputPort : Port, ICanBulidConnectionCache
    {
        protected sealed override PortDirection Direction => PortDirection.Input;
        public InputPort(string portName) : base(portName)
        {
        }
        /// <summary> 用于运行时和开发时构建，快速存取读取 </summary>
        [NonSerialized]
        internal HashSet<OutputPort> m_connections = new HashSet<OutputPort>();
        public IEnumerable<OutputPort> Connections => m_connections;
        public OutputPort Connection => m_connections.FirstOrDefault();

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
                return Connection.GetValue();
        }
    }
    [/* HideInInspector, */ Serializable]
    public class OutputPort : Port, ICanBulidConnectionCache
    {
        protected sealed override PortDirection Direction => PortDirection.Output;
        public OutputPort(string portName) : base(portName)
        {
        }
        /// <summary> 用于运行时和开发时构建，快速存取读取 </summary>
        [NonSerialized]
        internal HashSet<InputPort> m_connections = new HashSet<InputPort>();
        public IEnumerable<InputPort> Connections => m_connections;
        public InputPort Connection => m_connections.FirstOrDefault();
        /// <summary> 用于数据存取 序列化 </summary>
        [SerializeField, HideInInspector]
        internal List<string> m_connectionsGUID = new List<string>();

        public IEnumerable<string> connectionsGUID => m_connectionsGUID;

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
    [Serializable]
    public class InputPort<T> : InputPort
    {
        public T defaultValue;

        public InputPort(T defaultValue = default, string portName = "") : base(portName)
        {
            this.defaultValue = defaultValue;
        }

        public override object GetValue()
        {
            //InputPort取值规则：若自身无连接，使用默认值。若有连接，问OutputPort要值
            if (m_connections.Count == 0)
                return defaultValue;
            else
                return Connection.GetValue();
        }
        public static implicit operator T(InputPort<T> inputPort)
        {
            return (T)inputPort.GetValue();
        }
    }
    [Serializable]
    public class OutputPort<T> : OutputPort
    {
        public T defaultValue;
        public OutputPort(T defaultValue = default, string portName = "") :
            base(portName)
        {
            this.defaultValue = defaultValue;
        }
        public override object GetValue()
        {
            return base.GetValue() ?? default(T);
        }
        public static implicit operator T(OutputPort<T> outputPort)
        {
            return (T)outputPort.GetValue();
        }
    }
}