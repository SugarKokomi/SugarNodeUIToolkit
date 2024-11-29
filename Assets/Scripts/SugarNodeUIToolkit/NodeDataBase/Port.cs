using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

namespace SugarNode
{
    [HideInInspector, Serializable]
    public class Port
    {
        public Port(Func<List<Port>, List<Port>> connectionCondition = null)
        {
/* #if UNITY_EDITOR
            m_guid = GUID.Generate().ToString();
#endif */
            WhoCanConnectMe = connectionCondition ?? DefaultConnectCondition;
            GetValue = DefaultGetValue;
        }

        //自己的GUID，用于数据持久化存储
        [SerializeField/* , HideInInspector */]
        internal string m_guid;
        /// <summary> 用于Port的唯一标识 </summary>
        public string guid => m_guid;

        [SerializeField/* , HideInInspector */]
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
    }
    [HideInInspector, Serializable]
    public class InputPort : Port
    {
        /// <summary> 用于运行时和开发时构建，快速存取读取 </summary>
        [NonSerialized]
        internal HashSet<OutputPort> m_connections = new HashSet<OutputPort>();
        public IEnumerable<OutputPort> Connections => m_connections;
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