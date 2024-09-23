using UnityEditor;
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
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(NodeEditorSetting.UIElementFolderPath + "NodeGridElement.uss");
            styleSheets.Add(styleSheet);
        }

        // 右键节点创建栏
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // 添加Node抽象类下的所有子类到右键创建栏中
            {
                var types = TypeCache.GetTypesDerivedFrom<Node>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"{type.Name}", (a) => CreateNode(type));
                }
            }
        }

        void CreateNode(System.Type type)
        {
           
        }

        void CreateNodeView()
        {
            // 将对应节点UI添加到节点树视图上
            //AddElement();
        }

        // 只要节点树视图发生改变就会触发OnGraphViewChanged方法
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // 对所有删除进行遍历记录 只要视图内有元素删除进行判断
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    
                });
            }
            return graphViewChange;
        }
        internal void PopulateView()
        {
            // 在节点树视图重新绘制之前需要取消视图变更方法OnGraphViewChanged的订阅
            // 以防止视图变更记录方法中的信息是上一个节点树的变更信息
            graphViewChanged -= OnGraphViewChanged;
            // 清除之前渲染的graphElements图层元素
            DeleteElements(graphElements);
            // 在清除节点树视图所有的元素之后重新订阅视图变更方法OnGraphViewChanged
            graphViewChanged += OnGraphViewChanged;
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(
                endPort => endPort.direction != startPort.direction &&
                endPort.node != startPort.node
            ).ToList();
        }
    }
}