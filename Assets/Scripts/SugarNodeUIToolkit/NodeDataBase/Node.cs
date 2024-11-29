#define SugarNode
using UnityEngine;
using System.Collections.Generic;
using System;

namespace SugarNode
{
    public abstract class Node : ScriptableObject
    {
        [SerializeField, HideInInspector]
        internal Graph m_graph;
        public Graph graph => m_graph;
        [HideInInspector]
        public string guid;

        // [SerializeField, HideInInspector]
        private List<InputPort> m_inputs = new List<InputPort>();
        // [SerializeField, HideInInspector]
        private List<OutputPort> m_outputs = new List<OutputPort>();
        public IEnumerable<InputPort> Inputs => m_inputs;
        public IEnumerable<OutputPort> Outputs => m_outputs;
        protected void AddPort(Port port)
        {
            port.m_node = this;
#if UNITY_EDITOR
            port.m_guid = UnityEditor.GUID.Generate().ToString();
#endif
            if (port is InputPort inputPort)
            {
                m_inputs.Add(inputPort);
            }
            else if (port is OutputPort outputPort)
                m_outputs.Add(outputPort);
            else throw new TypeAccessException($"You can't add port > {port} < to the list;");
        }
        protected abstract void Init();//初始化节点、初始化端口、初始化方法
#if UNITY_EDITOR
        [HideInInspector]
        public Vector2 gridPos = Vector2.zero;
        internal void InitWhenCreate()
        {
            Init();
            Debug.Log($"创建了节点> {this.GetType()} <");
        }
#endif
        public virtual object GetValue(Port port) => null;
    }
}