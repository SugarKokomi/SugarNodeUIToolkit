using System;
using System.Collections.Generic;
using UnityEngine;

namespace SugarNode.Editor
{
    internal class NodeAttributeHandler
    {
        static NodeAttributeHandler m_instance;
        public static NodeAttributeHandler Instance { get { m_instance ??= new NodeAttributeHandler(); return m_instance; } }
        public Dictionary<Type, Dictionary<Type, List<Attribute>>> allAttributes;
        /// <summary> 通过CreateMenuAttribute获取右键创建的菜单路径 </summary>
        public string GetNodeCreateMenu(Type nodeType)
        {
            return nodeType.Name;
        }
        /// <summary> 通过NodeTitleAttribute获取节点标题文字 </summary>
        public string GetNodeTitle(Type nodeType)
        {
            return nodeType.Name;
        }
        /// <summary> 通过NodeWidthAttribute获取节点视图宽度 </summary>
        public float GetNodeWidth(Type nodeType)
        {
            return 100;
        }
        /// <summary> 通过NodeColorAttribute获取节点视图宽度 </summary>
        public Color GetNodeColor(Type nodeType)
        {
            return Color.white;
        }
        /// <summary> 通过OnlyAllowNodeAttribute获取Graph里只能创建哪些节点 </summary>
        public IEnumerable<Type> GetGraphAllowNodesType(Type graphType)
        {
            return default;
        }
        /// <summary> 通过RequiredNodeAttribute获取Graph里必须存在哪些节点 </summary>
        public IEnumerable<(Type,int)> GetGraphRequiredNodesType(Type graphType)
        {
            return default;
        }
    }
}