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
        private bool hadInitSelf = false, hadInitAll = false;
        [NonSerialized]
        private Dictionary<string, InputPort> m_allInputPortCache;
        [NonSerialized]
        private Dictionary<string, OutputPort> m_allOutputPortCache;
        /// <summary> 根据guid获取一个输入端口 </summary>
        public InputPort GetInputPort(string guid)
        {
            m_allInputPortCache.TryGetValue(guid, out var value);
            return value;
        }
        /// <summary> 根据guid获取一个输出端口 </summary>
        public OutputPort GetOutputPort(string guid)
        {
            m_allOutputPortCache.TryGetValue(guid, out var value);
            return value;
        }
        /// <summary> 构建一个OutputPort与其下所有的InputPort的互相连接关系 </summary>
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
        public void TryBuildAllRuntimeCache()
        {
            //逐级构建所有运行时缓存
            if (hadInitAll) return;
            hadInitAll = true;

            //构建Node与自己Port的互相引用
            foreach (ICanBulidConnectionCache nodeInitializer in nodes)
                nodeInitializer.InitCache();

            //构建Graph与Port之间的引用    
            ICanBulidConnectionCache graphInitializer = this;
            graphInitializer.InitCache();

            //初始化所有的Port
            foreach (ICanBulidConnectionCache portInitializer in m_allInputPortCache.Values)
                portInitializer.InitCache();
            foreach (ICanBulidConnectionCache portInitializer in m_allOutputPortCache.Values)
                portInitializer.InitCache();

            //初始化完毕，将两个Port之间互相引用起来
            foreach (var outputPort in m_allOutputPortCache.Values)
                BuildRuntimeConnection(outputPort);
        }
        public void TryClearAllRuntimeCache()
        {
            //逐级清除所有运行时缓存
            if (!hadInitAll) return;
            hadInitAll = false;

            //清除Port与Port之间的连接引用
            foreach (ICanClearConnectionCache inputPortDisposer in m_allInputPortCache.Values)
                inputPortDisposer.DisposeCache();
            foreach (ICanClearConnectionCache outputPortDisposer in m_allOutputPortCache.Values)
                outputPortDisposer.DisposeCache();

            //清除Node与Port之间的引用
            foreach (ICanBulidConnectionCache nodeDisposer in nodes)
                nodeDisposer.DisposeCache();

            //清除Graph与Port之间的引用
            ICanBulidConnectionCache graphDisposer = this;
            graphDisposer.DisposeCache();
        }
        void ICanBulidConnectionCache.InitCache()
        {
            if (hadInitSelf) return;
            hadInitSelf = true;
            m_allInputPortCache = new Dictionary<string, InputPort>();
            m_allOutputPortCache = new Dictionary<string, OutputPort>();
            //构建自己对Port的引用
            foreach (var node in nodes)
            {
                foreach (var input in node.Inputs)
                    m_allInputPortCache.AddOrSetValue(input.guid, input);
                foreach (var output in node.Outputs)
                    m_allOutputPortCache.AddOrSetValue(output.guid, output);
            }
        }
        void ICanClearConnectionCache.DisposeCache()
        {
            if (!hadInitSelf) return;
            hadInitSelf = false;
            //清除自己与Port之间的引用
            m_allInputPortCache.Clear();
            m_allOutputPortCache.Clear();
            m_allInputPortCache = null;
            m_allOutputPortCache = null;
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
            //初始化Node与Port的连接
            ICanBulidConnectionCache nodeInitializer = node;
            nodeInitializer.InitCache();
            //初始化Graph与Port的连接
            foreach (var input in node.Inputs)
                m_allInputPortCache.AddOrSetValue(input.guid, input);
            foreach (var output in node.Outputs)
                m_allOutputPortCache.AddOrSetValue(output.guid, output);
            // BuildRuntimeConnection(output);//新创建的节点，其所有连接一定是空的，就不用创建Port之间的互相连接了
        }
        private void RemoveEditorRuntimeConnections(Node node)
        {
            //清除Port与Port的连接、Graph与Port的连接
            foreach (var input in node.Inputs)
            {
                (input as ICanBulidConnectionCache).DisposeCache();
                m_allInputPortCache.Remove(input.guid);
            }
            foreach (var output in node.Outputs)
            {
                (output as ICanBulidConnectionCache).DisposeCache();
                m_allOutputPortCache.Remove(output.guid);
            }
            //清除Node与Port的连接
            ICanBulidConnectionCache nodeInitializer = node;
            nodeInitializer.DisposeCache();
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
                //删除自身所有端口的连接
                /*
                //这样遍历会对原始的遍历集合node.Outputs进行删除，会报错
                foreach (var output in node.Outputs)
                    foreach (var input in output.Connections)
                        DisConnectPort(output, input);
                foreach (var input in node.Inputs)
                    foreach (var output in input.Connections)
                        DisConnectPort(output, input); */

                //只需单向删除其他Node对自己的所有引用。自己对其他Node的引用会被删除节点的行为清除掉
                foreach (var output in node.Outputs)
                    foreach (var input in output.Connections)
                        input.m_connections.Remove(output);
                foreach (var input in node.Inputs)
                    foreach (var output in input.Connections)
                    {
                        output.m_connections.Remove(input);
                        output.m_connectionsGUID.Remove(input.guid);
                    }
                RemoveEditorRuntimeConnections(node);

                AssetDatabase.RemoveObjectFromAsset(node);
                AssetDatabase.SaveAssets();
                return true;
            }
            return false;
        }
    }
#endif
}