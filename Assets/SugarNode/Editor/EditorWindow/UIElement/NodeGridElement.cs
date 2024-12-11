using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using PortElement = UnityEditor.Experimental.GraphView.Port;
using System;

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
        }

        internal static GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    var inputNodeUI = edge.input.node as NodeElement;
                    var outputNodeUI = edge.output.node as NodeElement;
                    NodeEditorWindow.activeGraph.ConnectPort(
                        outputNodeUI.uiPairsPort[edge.output],
                        inputNodeUI.uiPairsPort[edge.input]);
                }
            }
            else if (graphViewChange.elementsToRemove != null)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    if (element is Edge edge)
                    {
                        var inputNodeUI = edge.input.node as NodeElement;
                        var outputNodeUI = edge.output.node as NodeElement;
                        NodeEditorWindow.activeGraph.DisConnectPort(
                            outputNodeUI.uiPairsPort[edge.output],
                            inputNodeUI.uiPairsPort[edge.input]);
                    }
                }
            }
            return graphViewChange;
        }

        // 右键创建节点，或者创建节点图
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (NodeEditorWindow.activeGraph)
            {
                var allNodesWhichCanCreateInThisGraph = NodeAttributeHandler.Instance.GetGraphAllowNodesType(NodeEditorWindow.activeGraph.GetType());
                Vector2 mousePositionInGridSpace = this.contentViewContainer.WorldToLocal(evt.mousePosition);
                foreach (var nodeType in allNodesWhichCanCreateInThisGraph)
                {
                    evt.menu.AppendAction(NodeAttributeHandler.Instance.GetNodeCreateMenu(nodeType),
                        _ => CreateNode(nodeType, mousePositionInGridSpace));
                }
                BuildOtherMenu(evt);
            }
            else
            {
                var allGraphWhichCanInstantiate = NodeAttributeHandler.Instance.GetAllGraphType();
                foreach (var graphType in allGraphWhichCanInstantiate)
                {
                    var attribute = NodeAttributeHandler.Instance.GetGraphCreateMenu(graphType);
                    evt.menu.AppendAction(attribute.menuName,
                        _ => CreateGraph(graphType, attribute.fileName));
                }
            }
        }
        private void BuildOtherMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("刷新", _ => RePaint());
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
        void CreateNode(Type type, Vector2 mousePositionInGridSpace)
        {
            if (NodeEditorWindow.activeGraph)
            {
                Node node = ScriptableObject.CreateInstance(type) as Node;
                node.name = NodeAttributeHandler.Instance.GetNodeDefaultName(type);
                node.gridPos = mousePositionInGridSpace;
                node.guid = GUID.Generate().ToString();
                NodeEditorWindow.activeGraph.AddNode(node);
                // 创建节点UI，并且添加到节点图中
                AddElement(new NodeElement(node));
            }
            else Debug.Log("请先选择激活一个Graph对象");
        }
        void CreateGraph(Type type, string fileName)
        {
            //获取当前Project窗口选中的一个文件夹
            string GetSelectObjectFolder()
            {
                var activeObject = Selection.activeObject;
                if (activeObject != null)
                {
                    string activePath = AssetDatabase.GetAssetPath(activeObject);
                    if (System.IO.Directory.Exists(activePath))
                        return activePath;
                    else // 如果选中的对象不是一个文件夹，找到其所在的文件夹
                        return System.IO.Path.GetDirectoryName(activePath);
                }
                else return "Assets";
            }
            // ProjectWindowUtil.CreateAsset(graph, $"Assets/{fileName}.asset");//这个是直接创建物体，不会挂起线程
            string path = EditorUtility.SaveFilePanel($"保存{fileName}", GetSelectObjectFolder(), $"{fileName}", "asset");//这个玩意儿会把进程暂停在这句
            if (string.IsNullOrEmpty(path))
                return;
            path = FileUtil.GetProjectRelativePath(path);
            var graph = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(graph, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            NodeEditorWindow.Instance.TrySwitchGraphAndRepaint(graph as Graph);
        }
        void DeleteNode(Node node)
        {
            NodeEditorWindow.activeGraph.DeleteNode(node);
        }
        public void RePaint()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            if (NodeEditorWindow.activeGraph)
            {
                List<NodeElement> nodeElements = new List<NodeElement>();
                foreach (var node in NodeEditorWindow.activeGraph.Nodes)
                {
                    var nodeElement = new NodeElement(node);
                    AddElement(nodeElement);//绘制所有的Node
                    nodeElements.Add(nodeElement);//添加到缓存
                }
                //等节点全部绘制完后，再绘制线条
                foreach (var nodeElement in nodeElements)//遍历每个节点
                {
                    IEnumerable<PortElement> outputPorts = nodeElement.uiPairsPort.Keys.Where(port => port.direction == Direction.Output);//获取UI上所有的输出端口
                    foreach (var outputPort in outputPorts)//遍历节点UI的Output Port，绘制每一条连接
                    {
                        IEnumerable<string> guids = NodeEditorWindow.activeGraph.GetOutputPort(nodeElement.uiPairsPort[outputPort]).connectionsGUID;//获取连接的每个输入端口的guid
                        foreach (var guid in guids)//遍历每个与自己（输出端口）所连接的输入端口,用一条线连接上
                        {
                            Edge edge = outputPort.ConnectTo(NodeElement.uiPairsPortReverse[guid]);
                            AddElement(edge);
                        }
                    }
                }
            }
            graphViewChanged += OnGraphViewChanged;

        }
        /// <summary> 获取Port的可连接Port列表 </summary>
        public override List<PortElement> GetCompatiblePorts(PortElement startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(
                endPort => endPort.direction != startPort.direction &&
                endPort.node != startPort.node
            ).ToList();
        }
    }
}