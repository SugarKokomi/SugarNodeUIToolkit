#define SugarNode
using UnityEngine;
using System.Collections.Generic;
using System;

namespace SugarNode
{
    public abstract partial class Node : ScriptableObject, ICanBulidConnectionCache
    {
        [SerializeField, HideInInspector]
        internal Graph m_graph;
        public Graph graph => m_graph;

        [HideInInspector]
        public string guid;
        internal bool hadInit = false;

#if UNITY_EDITOR
        [HideInInspector]
        public Vector2 gridPos = Vector2.zero;
#endif
        internal List<InputPort> m_inputs = new List<InputPort>();
        internal List<OutputPort> m_outputs = new List<OutputPort>();
        public IEnumerable<InputPort> Inputs { get { InitCache(); return m_inputs; } }
        public IEnumerable<OutputPort> Outputs { get { InitCache(); return m_outputs; } }
        public void InitCache()
        {
            if (hadInit) return;
            hadInit = true;
            Init();
        }
        public void DisposeCache()
        {
            if (!hadInit) return;
            hadInit = false;
            m_inputs.Clear();
            m_outputs.Clear();
        }
        protected void InitPort(Port port)
        {
#if UNITY_EDITOR            
            if (!port.m_node)//若m_node为null，说明该Port是new出来的。若不为null，说明是Unity序列化出来的
            {
                port.m_node = this;
                port.m_guid = UnityEditor.GUID.Generate().ToString();
            }
#endif

            if (port is InputPort inputPort)
                m_inputs.Add(inputPort);
            else if (port is OutputPort outputPort)
                m_outputs.Add(outputPort);
            else throw new TypeAccessException($"You can't add port > {port} < to the list;");
        }
        protected abstract void Init();//初始化节点、初始化端口、初始化方法
        public virtual object GetValue(Port port) => null;
        protected virtual string ShowTips() => string.Empty;
    }
#if UNITY_EDITOR
    partial class Node
    {
        public string GetTips() => ShowTips();
    }
#endif
}