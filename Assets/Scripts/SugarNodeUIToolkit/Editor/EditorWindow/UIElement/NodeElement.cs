using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using PortElement = UnityEditor.Experimental.GraphView.Port;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

namespace SugarNode.Editor
{
    public class NodeElement : UnityEditor.Experimental.GraphView.Node
    {
        public static Action<NodeElement> OnNodeSelected;
        internal Node node;
        internal Dictionary<PortElement, string> uiPairsPort;
        internal static Dictionary<string, PortElement> uiPairsPortReverse;//前者的反向字典
        public NodeElement(Node node) : base("Assets/Scripts/SugarNodeUIToolkit/Editor/Resources/NodeElement.uxml")
        {
            this.node = node;
            this.title = node.name;
            this.viewDataKey = node.guid;

            style.left = node.gridPos.x;
            style.top = node.gridPos.y;

            uiPairsPort = new Dictionary<PortElement, string>();
            uiPairsPortReverse ??= new Dictionary<string, PortElement>();

            InitPort();
            SetTipsText();
            SetWidth();
            SetColor();
        }
        private void InitPort()
        {
            foreach (var inputPort in node.Inputs)
            {
                var portUI = base.InstantiatePort(
                    Orientation.Horizontal,
                    Direction.Input,
                    PortElement.Capacity.Multi,
                    GetPortType(inputPort)
                );
                portUI.portName = inputPort.PortName;
                uiPairsPort.Add(portUI, inputPort.guid);
                if (uiPairsPortReverse.ContainsKey(inputPort.guid))
                    uiPairsPortReverse[inputPort.guid] = portUI;
                else uiPairsPortReverse.Add(inputPort.guid, portUI);
                inputContainer.Add(portUI);
            }
            foreach (var outputPort in node.Outputs)
            {
                var portUI = base.InstantiatePort(
                    Orientation.Horizontal,
                    Direction.Output,
                    PortElement.Capacity.Multi,
                    GetPortType(outputPort)
                );
                portUI.portName = outputPort.PortName;
                uiPairsPort.Add(portUI, outputPort.guid);
                if (uiPairsPortReverse.ContainsKey(outputPort.guid))
                    uiPairsPortReverse[outputPort.guid] = portUI;
                else uiPairsPortReverse.Add(outputPort.guid, portUI);
                outputContainer.Add(portUI);
            }
        }
        private static Type GetPortType(Port port)
        {
            Type objectType = port.GetType();
            // 检查该类型是否是泛型类 Port<T> 的实例
            if (objectType.IsGenericType)
            {
                Type tType = objectType.GetGenericTypeDefinition();
                if (tType == typeof(InputPort<>) || tType == typeof(OutputPort<>))
                    return objectType.GetGenericArguments()[0];
                else return null;
            }
            else return null;
        }
        public override void SetPosition(Rect newPos)
        {
            // 将视图中节点位置设置为最新位置newPos
            base.SetPosition(newPos);
            // 将最新位置记录到运行时节点树中持久化存储
            node.gridPos.x = newPos.xMin;
            node.gridPos.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }
        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
            // NodeEditorWindow.Instance.SetSelectionNoEvent(this.node);
            Selection.activeObject = node;
        }
        private void SetTipsText()
        {
            var titleTips = this.Q<Label>("title-Tips");
            titleTips.text = node.GetTips();
        }
        private void SetWidth()
        {
            var root = this.Q("BackGround");
            float minWidth = NodeAttributeHandler.Instance.GetNodeWidth(node.GetType());
            root.style.minWidth = minWidth > 0 ? minWidth : StyleKeyword.Auto;
        }
        private void SetColor()
        {
            var root = this.Q("BackGround");
            var title = this.Q("title-label");
            var color = NodeAttributeHandler.Instance.GetNodeColor(node.GetType());
            var bgColor = color * 0.2f;
            bgColor.a = 1;
            var titleColor = color * 0.5f;
            titleColor.a = 1;
            root.style.borderTopColor = titleColor;
            root.style.borderBottomColor = titleColor;
            root.style.borderLeftColor = titleColor;
            root.style.borderRightColor = titleColor;
            title.style.backgroundColor = titleColor;
            root.style.backgroundColor = bgColor;
        }
    }
}