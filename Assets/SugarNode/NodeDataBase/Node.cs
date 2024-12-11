#define SugarNode
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SugarNode
{
    public abstract partial class Node : ScriptableObject, ICanBulidConnectionCache
    {
        [SerializeField, HideInInspector]
        internal Graph m_graph;
        public Graph graph => m_graph;

        [HideInInspector]
        public string guid;
        [NonSerialized]
        internal bool hadInitSelf = false;

#if UNITY_EDITOR
        [HideInInspector]
        public Vector2 gridPos = Vector2.zero;
#endif
        [NonSerialized] internal List<InputPort> m_inputs;
        [NonSerialized] internal List<OutputPort> m_outputs;
        public IEnumerable<InputPort> Inputs => m_inputs;
        public InputPort DefaultInput => m_inputs.FirstOrDefault();
        public IEnumerable<OutputPort> Outputs => m_outputs;
        public OutputPort DefaultOutput => m_outputs.FirstOrDefault();
        void ICanBulidConnectionCache.InitCache()
        {
            if (hadInitSelf) return;
            hadInitSelf = true;
            m_inputs = new List<InputPort>();
            m_outputs = new List<OutputPort>();
            Init();
        }
        void ICanClearConnectionCache.DisposeCache()
        {
            if (!hadInitSelf) return;
            hadInitSelf = false;
            m_inputs.Clear();
            m_outputs.Clear();
            m_inputs = null;
            m_outputs = null;
        }
        protected void InitPort(Port port)
        {
#if UNITY_EDITOR            
            if (string.IsNullOrEmpty(port.m_guid))//若guid为null，说明该Port是new出来的。若不为null，说明是Unity序列化出来的
            {
                port.m_guid = UnityEditor.GUID.Generate().ToString();
            }
#endif
            port.m_node = this;
            if (port is InputPort inputPort)
                m_inputs.Add(inputPort);
            else if (port is OutputPort outputPort)
                m_outputs.Add(outputPort);
            else throw new TypeAccessException($"You can't add port > {port} < to the list;");
        }
        protected abstract void Init();//初始化节点、初始化端口、初始化方法
        public virtual object GetValue(OutputPort port) => null;
        protected virtual string ShowTips() => string.Empty;
    }
#if UNITY_EDITOR
    partial class Node
    {
        public string GetTips() => ShowTips();
    }
#endif
}