using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace SugarNode.Editor
{
    internal class NodeAttributeHandler
    {
        static NodeAttributeHandler m_instance;
        internal static NodeAttributeHandler Instance { get { m_instance ??= new NodeAttributeHandler(); return m_instance; } set { m_instance = value; } }
        private Dictionary<Type, string> createMenuAttributes = new Dictionary<Type, string>();
        private Dictionary<Type, string> nodeNameAttributes = new Dictionary<Type, string>();
        private Dictionary<Type, float> nodeWidthAttributes = new Dictionary<Type, float>();
        private Dictionary<Type, Color> nodeColorAttributes = new Dictionary<Type, Color>();
        private Dictionary<Type, HashSet<Type>> graphAllowNodesAttributes = new Dictionary<Type, HashSet<Type>>();
        private Dictionary<Type, List<(Type, uint)>> graphRequiredNodesAttributes = new Dictionary<Type, List<(Type, uint)>>();
        /// <summary> 通过CreateMenuAttribute获取右键创建的菜单路径 </summary>
        internal string GetNodeCreateMenu(Type nodeType)
        {
            if (createMenuAttributes.TryGetValue(nodeType, out var path))
                return path;
            else
            {
                var attribute = nodeType.GetCustomAttribute<CreateMenuAttribute>();
                createMenuAttributes.Add(nodeType, attribute?.menuPath ?? nodeType.Name);
                return createMenuAttributes[nodeType];
            }

        }
        /// <summary> 通过NodeTitleAttribute获取节点标题文字 </summary>
        internal string GetNodeDefaultName(Type nodeType)
        {
            if (nodeNameAttributes.TryGetValue(nodeType, out var name))
                return name;
            else
            {
                var attribute = nodeType.GetCustomAttribute<NodeDefaultNameAttribute>();
                nodeNameAttributes.Add(nodeType, attribute?.name ?? nodeType.Name);
                return nodeNameAttributes[nodeType];
            }
        }
        /// <summary> 通过NodeWidthAttribute获取节点视图宽度 </summary>
        internal float GetNodeWidth(Type nodeType)
        {
            if (nodeWidthAttributes.TryGetValue(nodeType, out var width))
                return width;
            else
            {
                var attribute = nodeType.GetCustomAttribute<NodeWidthAttribute>();
                if (attribute != null)
                    nodeWidthAttributes.Add(nodeType, attribute.width);
                else nodeWidthAttributes.Add(nodeType, -1);
                return nodeWidthAttributes[nodeType];
            }
        }
        /// <summary> 通过NodeColorAttribute获取节点视图宽度 </summary>
        internal Color GetNodeColor(Type nodeType)
        {
            if (nodeColorAttributes.TryGetValue(nodeType, out var color))
                return color;
            else
            {
                var attribute = nodeType.GetCustomAttribute<NodeColorAttribute>();
                nodeColorAttributes.Add(nodeType, attribute?.color ?? Color.white);
                return nodeColorAttributes[nodeType];
            }
        }
        /// <summary> 通过OnlyAllowNodeAttribute获取Graph里只能创建哪些节点。 </summary>
        /// 会忽略抽象类，会计算Graph挂载的全部[OnlyAllowNode(...)]标签
        internal IEnumerable<Type> GetGraphAllowNodesType(Type graphType)
        {
            if (graphAllowNodesAttributes.TryGetValue(graphType, out var nodeTypes))
                return nodeTypes;
            else
            {
                var attributes = graphType.GetCustomAttributes<OnlyAllowNodeAttribute>();
                if (attributes != null && attributes.Count() != 0)
                {
                    HashSet<Type> allowNodes = new HashSet<Type>();
                    foreach (var attribute in attributes)
                    {
                        var allChildTypes = TypeCache.GetTypesDerivedFrom(attribute.nodeType);
                        var allowTypes = allChildTypes.Where(type => !type.IsAbstract).ToHashSet();
                        allowNodes.UnionWith(allowTypes);
                    }
                    graphAllowNodesAttributes.Add(graphType, allowNodes);
                }
                else
                {
                    var allChildTypes = TypeCache.GetTypesDerivedFrom<Node>();
                    var allowTypes = allChildTypes.Where(type => !type.IsAbstract).ToHashSet();
                    graphAllowNodesAttributes.Add(graphType, allowTypes);
                }
                return graphAllowNodesAttributes[graphType];
            }
        }
        /// <summary> 通过RequiredNodeAttribute获取Graph里必须存在哪些节点 </summary>
        internal IEnumerable<(Type, uint)> GetGraphRequiredNodesType(Type graphType)
        {
            if (graphRequiredNodesAttributes.TryGetValue(graphType, out var nodesInfo))
                return nodesInfo;
            else
            {
                var attributes = graphType.GetCustomAttributes<RequiredNodeAttribute>();
                if (attributes != null && attributes.Count() != 0)
                {
                    List<(Type, uint)> requiredNodes = new List<(Type, uint)>();
                    foreach (var attribute in attributes)
                        requiredNodes.Add((attribute.nodeType, attribute.minNum));
                    graphRequiredNodesAttributes.Add(graphType, requiredNodes);
                }
                else
                {
                    graphRequiredNodesAttributes.Add(graphType, null);
                }
                return graphRequiredNodesAttributes[graphType];
            }
        }
    }
}