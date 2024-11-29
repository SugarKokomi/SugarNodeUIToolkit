using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace SugarNode
{
    public abstract partial class Graph : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private List<Node> nodes = new List<Node>();//Graph所管理的所有节点
        public IEnumerable<Node> Nodes => nodes;
        private bool hadInit = false;
        [NonSerialized]
        internal Dictionary<string, InputPort> m_allInputPortCache = new Dictionary<string, InputPort>();
        [NonSerialized]
        internal Dictionary<string, OutputPort> m_allOutputPortCache = new Dictionary<string, OutputPort>();
        public InputPort GetInputPort(string guid)
        {
            TryBulidAllRuntimeConnections();
            m_allInputPortCache.TryGetValue(guid, out var value);
            return value;
        }
        public OutputPort GetOutputPort(string guid)
        {
            TryBulidAllRuntimeConnections();
            m_allOutputPortCache.TryGetValue(guid, out var value);
            return value;
        }
        // internal void AddPortReference()
        /// <summary> 构建一个运行时的节点连接关系 </summary>
        public void BuildRuntimeConnections(OutputPort outputPort)
        {
            foreach (var inputPortGUID in outputPort.m_connectionsGUID)
            {
                var inputPort = GetInputPort(inputPortGUID);
                if (inputPort == null)
                {
                    Debug.LogWarning($"构建节点连接信息失败，疑似GUID丢失：\n{inputPortGUID}");
                    continue;
                }
                //建立运行时双方的连接
                inputPort.m_connections.Add(outputPort);
                outputPort.m_connections.Add(inputPort);
            }
        }
        private void BulidAllRuntimeConnections()
        {
            //添加上自身节点的全部端口的引用
            foreach (var node in nodes)
            {
                foreach (var input in node.Inputs)
                    m_allInputPortCache.TryAdd(input.guid, input);
                foreach (var output in node.Outputs)
                    m_allOutputPortCache.TryAdd(output.guid, output);
            }
            //遍历所有输出-输入关系，构建运行时的连接缓存
            foreach (var kvp in m_allOutputPortCache)
                BuildRuntimeConnections(kvp.Value);
        }
        public void TryBulidAllRuntimeConnections()
        {
            if (!hadInit)
            {
                BulidAllRuntimeConnections();
                hadInit = true;
            }
        }
    }
#if UNITY_EDITOR
    public partial class Graph
    {
        public void ConnectPort(string outputPortGUID, string inputPortGUID)
        {
            var output = GetOutputPort(outputPortGUID);
            var input = GetInputPort(inputPortGUID);
            if (input == null || output == null) return;
            ConnectPort(output, input);
        }
        public void ConnectPort(OutputPort outputPort, InputPort inputPort)
        {
            if (!outputPort.m_connectionsGUID.Contains(inputPort.guid))
                outputPort.m_connectionsGUID.Add(inputPort.guid);
            outputPort.m_connections.Add(inputPort);
            inputPort.m_connections.Add(outputPort);
        }
        public void DisConnectPort(string outputPortGUID, string inputPortGUID)
        {
            var output = GetOutputPort(outputPortGUID);
            var input = GetInputPort(inputPortGUID);
            if (input == null || output == null) return;
            DisConnectPort(output, input);
        }
        public void DisConnectPort(OutputPort outputPort, InputPort inputPort)
        {
            outputPort.m_connectionsGUID.Remove(inputPort.guid);
            outputPort.m_connections.Remove(inputPort);
            inputPort.m_connections.Remove(outputPort);
        }
        /// <summary> 每次创建节点时，需要单独构建自己的连接缓存 </summary>
        private void BulidEditorRuntimeConnections(Node node)
        {
            foreach (var input in node.Inputs)
                m_allInputPortCache.TryAdd(input.guid, input);
            foreach (var output in node.Outputs)
            {
                m_allOutputPortCache.TryAdd(output.guid, output);
                BuildRuntimeConnections(output);
            }
        }
        public void AddNode(Node node)
        {
            nodes.Add(node);
            node.m_graph = this;
            node.InitWhenCreate();
            BulidEditorRuntimeConnections(node);
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
    }
#endif
}