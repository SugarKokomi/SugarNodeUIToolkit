using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace SugarNode.Editor
{
    public class NodeGridElement : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NodeGridElement, UxmlTraits> { }
        public NodeGridElement()
        {
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());// 添加视图缩放
            this.AddManipulator(new ContentDragger());// 添加视图拖拽
            this.AddManipulator(new SelectionDragger());// 添加选中对象拖拽
            this.AddManipulator(new RectangleSelector());// 添加框选
            var styleSheet = Resources.Load<StyleSheet>("NodeEditorWindow_StyleSheet");
            styleSheets.Add(styleSheet);
            NodeEditorWindow.Window.nodeGridElement = this;
        }
        // 右键创建节点
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (NodeEditorWindow.activeGraph)
            {
                var types = TypeCache.GetTypesDerivedFrom<Node>();
                Vector2 mousePositionInGridSpace = this.contentViewContainer.WorldToLocal(evt.mousePosition);
                foreach (var type in types)
                {
                    if (type.IsAbstract) continue;//无法new出来抽象类、静态类
                    evt.menu.AppendAction($"{type.Name}", (a) => CreateNode(type, mousePositionInGridSpace));
                }
            }
            else Debug.Log("请先选择激活一个Graph对象");
        }
        //Delect键删除节点
        public override EventPropagation DeleteSelection()
        {
            foreach (GraphElement element in selection.Cast<GraphElement>())
            {
                if (element is NodeElement nodeUI)
                    DeleteNode(nodeUI.node);
            }
            return base.DeleteSelection();
        }
        void CreateNode(System.Type type, Vector2 mousePositionInGridSpace)
        {
            if (NodeEditorWindow.activeGraph)
            {
                Node node = ScriptableObject.CreateInstance(type) as Node;
                node.name = type.Name;
                node.gridPos = mousePositionInGridSpace;
                node.guid = GUID.Generate().ToString();
                NodeEditorWindow.activeGraph.AddNode(node);
                // 创建节点UI，并且添加到节点图中
                AddElement(new NodeElement(node));
            }
            else Debug.Log("请先选择激活一个Graph对象");
        }
        void DeleteNode(Node node)
        {
            NodeEditorWindow.activeGraph.DeleteNode(node);
        }
        public void RePaint()
        {
            DeleteElements(graphElements);
            if (NodeEditorWindow.activeGraph)
            {
                foreach (var node in NodeEditorWindow.activeGraph.Nodes)
                {
                    AddElement(new NodeElement(node));
                }
            }
        }
        /// <summary> 获取Port的可连接Port列表 </summary>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(
                endPort => endPort.direction != startPort.direction &&
                endPort.node != startPort.node
            ).ToList();
        }
    }
}