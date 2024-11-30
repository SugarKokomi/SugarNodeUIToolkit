using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace SugarNode
{
    public abstract partial class Graph : ScriptableObject, ICanBulidConnectionCache
    {
        [SerializeField, HideInInspector]
        private List<Node> nodes = new List<Node>();//Graph所管理的所有节点
        public IEnumerable<Node> Nodes => nodes;
        [NonSerialized]
        private bool hadInit = false;
        [NonSerialized]
        internal Dictionary<string, InputPort> m_allInputPortCache = new Dictionary<string, InputPort>();
        [NonSerialized]
        internal Dictionary<string, OutputPort> m_allOutputPortCache = new Dictionary<string, OutputPort>();
        /// <summary> 根据guid获取一个输入端口 </summary>
        public InputPort GetInputPort(string guid)
        {
            InitCache();
            m_allInputPortCache.TryGetValue(guid, out var value);
            return value;
        }
        /// <summary> 根据guid获取一个输出端口 </summary>
        public OutputPort GetOutputPort(string guid)
        {
            InitCache();
            m_allOutputPortCache.TryGetValue(guid, out var value);
            return value;
        }
        //构建一个运行时的节点连接关系
        private void BuildRuntimeConnection(OutputPort outputPort)
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
        /// <summary> 清除全部的连接缓存 </summary>
        public void TryClearAllRuntimeCache()
        {
            //逐级清除所有运行时缓存
            //清除Port与Port之间的连接引用
            foreach (ICanClearConnectionCache inputCache in m_allInputPortCache.Values)
                inputCache.DisposeCache();
            foreach (ICanClearConnectionCache outputCache in m_allOutputPortCache.Values)
                outputCache.DisposeCache();

            //清除Node与Port之间的引用
            foreach (ICanBulidConnectionCache nodeCache in nodes)
                nodeCache.DisposeCache();

            (this as ICanClearConnectionCache).DisposeCache();
        }

        public void InitCache()
        {
            if (hadInit) return;
            hadInit = true;
            //构建Graph与Port之间的引用
            foreach (var node in nodes)
            {
                foreach (var input in node.Inputs)
                    m_allInputPortCache.AddOrSetValue(input.guid, input);
                foreach (var output in node.Outputs)
                    m_allOutputPortCache.AddOrSetValue(output.guid, output);
            }
            //遍历所有OutputPort，构建与InputPort之间的互相引用（连接信息存储在OutputPort里）
            foreach (var kvp in m_allOutputPortCache)
                BuildRuntimeConnection(kvp.Value);
        }
        void ICanClearConnectionCache.DisposeCache()
        {
            if (!hadInit) return;
            hadInit = false;
            //清除自己与Port之间的引用
            m_allInputPortCache.Clear();
            m_allOutputPortCache.Clear();
        }
    }
#if UNITY_EDITOR
    partial class Graph
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
            EditorUtility.SetDirty(this);
            // AssetDatabase.SaveAssets();//可用于AutoSave
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
            EditorUtility.SetDirty(this);
            // AssetDatabase.SaveAssets();
        }
        /// <summary> 每次创建节点时，需要单独构建自己的连接缓存 </summary>
        private void BulidEditorRuntimeConnections(Node node)
        {
            foreach (var input in node.Inputs)
                m_allInputPortCache.AddOrSetValue(input.guid, input);
            foreach (var output in node.Outputs)
            {
                m_allOutputPortCache.AddOrSetValue(output.guid, output);
                BuildRuntimeConnection(output);
            }
        }
        public void AddNode(Node node)
        {
            nodes.Add(node);
            node.m_graph = this;
            BulidEditorRuntimeConnections(node);
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            Undo.RegisterCreatedObjectUndo(node, $"创建{node.name}");
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